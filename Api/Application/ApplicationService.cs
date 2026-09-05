using System.Text.RegularExpressions;
using Api.Application.Interfaces;
using Api.Domain;
using Api.Exceptions;
using Api.Infrastructure.Github;
using Humanizer;

namespace Api.Application;

public class ApplicationService (ICustomerRepository customerRepository, IApplicationRepository applicationRepository, IServerConnection server, IFilePusher filePusher, GithubDockerComposeFileFetcher dockerComposeFileFetcher, ITagRepository tagRepository, GithubWorkflowRepository workflowRepository, IReleaseRepository releaseRepository, ILogger<ApplicationService> logger)
{
    public async Task Deploy(int applicationId, Tag tag)
    {
        logger.LogInformation("Beginning deployment...");
        
        var customer = await customerRepository.GetCustomerByApplicationId(applicationId);

        if (customer == null)
            throw new NotFoundException($"Could not find customer from application id. Id = {applicationId}.");
        
        var application = customer.Applications.FirstOrDefault(app => app.Id == applicationId);
        
        if (application == null)
            throw new NotFoundException($"Could not find application. Id = {applicationId}.");
        
        var workflow = await workflowRepository.GetWorkflow(application.Repository, tag);
        
        if (workflow == null)
            throw new NotFoundException($"Could not find workflow for tag {tag.Name}.");
        
        if (!workflow.IsComplete || !workflow.IsSuccessful)
            throw new Exception("Workflow is not completed successfully.");
        
        var release = await releaseRepository.GetRelease(application.Repository, tag.Name);

        if (release != null)
            logger.LogDebug("Tag {Tag} has been released before",  tag.Name);

        await PushAndDeploy(application, customer, tag.Name);

        if (release != null)
            await releaseRepository.SetLatestRelease(application.Repository, release.Id);
        else
            await releaseRepository.CreateRelease(application.Repository, tag.Name);

        logger.LogInformation("Deploy succeeded for {ApplicationName}", application.Name);
    }

    private async Task PushAndDeploy(Domain.Application application, Customer customer, string tagName)
    {
        logger.LogInformation("Pushing deployment configuration to server...");

        var composeFile = await GetComposeFile(application.Repository, tagName);
        var customerName = customer.Name.Kebaberize();
        var containerNamePrefix =  $"{customerName}-{application.Name.Kebaberize()}";
        var remoteDeploymentPath = $"{customerName}/{application.Name.Kebaberize()}";

        var remoteDir = await filePusher.Push(application.Server, composeFile, remoteDeploymentPath);
        
        logger.LogInformation("Deployment configuration pushed");
        
        logger.LogInformation("Deploying application...");

        var deployResult = await server.Deploy(application.Server, containerNamePrefix, remoteDir);

        if (deployResult.IsFailure)
        {
            logger.LogError("Application deploy failed: {Error}", deployResult.Error);
            throw new InvalidOperationException(deployResult.Error);
        }

        logger.LogInformation("Application deployed");
    }

    public async Task<List<Tag>> GetTags(int id)
    {
        var application = await applicationRepository.GetApplication(id);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var tags = await tagRepository.GetTags(application.Repository);

        return tags;
    }
    
    public async Task<Tag> CreateTag(int id)
    {
        logger.LogInformation("Creating tag...");
        var application = await applicationRepository.GetApplication(id);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var tag = await tagRepository.CreateTag(application.Repository);
        
        logger.LogInformation("Tag {TagName} created for {ApplicationName}", tag.Name, application.Name);

        return tag;
    }
    
    public async Task<Workflow> GetWorkflow(int id, Tag tag)
    {
        var application = await applicationRepository.GetApplication(id);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var workflow = await workflowRepository.GetWorkflow(application.Repository, tag);
        
        if  (workflow == null)
            throw new NotFoundException($"Workflow for tag {tag.Name} not found.");
        
        return workflow;
    }
    
    public async Task<List<Component>> GetContainers(int applicationId)
    {
        var customer = await customerRepository.GetCustomerByApplicationId(applicationId);

        if (customer == null)
            throw new NotFoundException($"Could not find customer from application id. Id = {applicationId}.");

        var application = customer.Applications.FirstOrDefault(app => app.Id == applicationId);

        if (application == null)
            throw new NotFoundException($"Could not find application. Id = {applicationId}.");

        var componentsResult = await server.GetComponents(application.Server);

        if (componentsResult.IsFailure)
            return [];

        var containerNamePrefix = ComponentNaming.GetContainerNamePrefix(customer.Name, application.Name);

        return componentsResult.Value
            .Where(component => component.ContainerName.StartsWith(containerNamePrefix))
            .Select(component =>
            {
                var shortName = ComponentNaming.GetShortComponentName(component.ContainerName, containerNamePrefix);
                return component with { Name = shortName };
            })
            .ToList();
    }

    public async Task<List<Domain.Application>> GetAll()
    {
        return await applicationRepository.GetApplications();
    }
    
    public async Task<Domain.Application> Get(int id)
    {
        var application = await applicationRepository.GetApplication(id);

        if (application is null)
            throw new NotFoundException("No application found.");

        return application;
    }

    public async Task<Release> GetLatestRelease(int id)
    {
        var application = await applicationRepository.GetApplication(id);

        if (application is null)
            throw new NotFoundException("No application found.");

        var release = await releaseRepository.GetLatestRelease(application.Repository);
        
        if (release == null)
            throw new NotFoundException("No latest release.");
        
        return release;
    }

    public async Task<Release> GetRelease(int id, string tagName)
    {
        var application = await applicationRepository.GetApplication(id);

        if (application is null)
            throw new NotFoundException("No application found.");

        var release = await releaseRepository.GetRelease(application.Repository, tagName);
        
        if (release == null)
            throw new NotFoundException("Could not find release for tag {tagName}.");
        
        return  release;
    }

    private async Task<string> GetComposeFile(string repository, string tagName)
    {
        var composeFile = await dockerComposeFileFetcher.GetComposeFileAsync(repository, tagName);
        var resolvedComposeFile = ResolveComposeFile(composeFile, repository, tagName);

        return resolvedComposeFile;
    }

    private static readonly Regex ImagePlaceholderPattern = new(@"\$\{(\w+)\}", RegexOptions.Compiled);

    private string ResolveComposeFile(string composeYaml, string repository, string tag)
    {
        var expectedComponents = ImagePlaceholderPattern
            .Matches(composeYaml)
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();

        if (expectedComponents.Count == 0)
            return composeYaml;

        var imageRefs = expectedComponents.ToDictionary(
            component => component,
            component => $"ghcr.io/{Config.Organization}/{repository}-{component}:{tag}");

        var resolvedYaml = composeYaml;
        foreach (var (component, imageRef) in imageRefs)
            resolvedYaml = resolvedYaml.Replace($"${{{component}}}", imageRef);

        if (resolvedYaml.Contains("${"))
            throw new InvalidOperationException($"Unresolved placeholder remains in compose file for '{repository}'.");

        return resolvedYaml;
    }
}