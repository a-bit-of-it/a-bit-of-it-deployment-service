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
        
        var workflow = await workflowRepository.GetWorkflow(application.Repository, tag.CommitSha);
        
        if (workflow == null)
            throw new NotFoundException($"Could not find workflow for tag {tag.Name}.");
        
        if (workflow is { IsComplete: false, IsSuccessful: false })
            throw new Exception("Workflow is not completed successfully.");
        
        var composeFile = await GetComposeFile(application.Repository, tag.Name);
        var customerName = customer.Name.Kebaberize();
        var remoteDeploymentPath = $"{customerName}/{application.Name.Kebaberize()}";
        
        var remoteDir = await filePusher.Push(application.Server, composeFile, remoteDeploymentPath);

        await server.Deploy(application.Server, customerName, remoteDir);
        
        await releaseRepository.CreateRelease(application.Repository, tag.Name);
        
        logger.LogInformation("Deploy succeeded for {ApplicationName}", application.Name);
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
        var application = await applicationRepository.GetApplication(id);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var tag = await tagRepository.CreateTag(application.Repository);

        return tag;
    }
    
    public async Task<Workflow> GetWorkflow(int id, string commitSha)
    {
        var application = await applicationRepository.GetApplication(id);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var workflow = await workflowRepository.GetWorkflow(application.Repository, commitSha);
        
        if  (workflow == null)
            throw new NotFoundException($"Workflow for tag {commitSha} not found.");
        
        return workflow;
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

    public async Task<Release?> GetLatestRelease(int id)
    {
        var application = await applicationRepository.GetApplication(id);

        if (application is null)
            throw new NotFoundException("No application found.");

        return await releaseRepository.GetLatestRelease(application.Repository);
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
            throw new InvalidOperationException($"No image placeholders found in compose file for '{repository}'.");

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