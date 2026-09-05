using System.Text.RegularExpressions;
using Api.Application.Interfaces;
using Api.Domain;
using Api.Exceptions;
using Api.Infrastructure.Github;
using Humanizer;

namespace Api.Application;

public class DeploymentService (ICustomerRepository customerRepository, IServerConnection server, IDeploymentConfigFetcher deploymentConfigFetcher, IWorkflowRepository workflowRepository, IReleaseRepository releaseRepository, ILogger<DeploymentService> logger)
{
    public async Task Deploy(int applicationId, Tag tag)
    {
        logger.LogInformation("Deploying application...");

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

        await PushAndPromote(application, customer, tag.Name);

        if (release != null)
            await releaseRepository.SetLatestRelease(application.Repository, release.Id);
        else
            await releaseRepository.CreateRelease(application.Repository, tag.Name);

        logger.LogInformation("Application deployed");
    }

    private async Task PushAndPromote(Domain.Application application, Customer customer, string tagName)
    {
        logger.LogInformation("Pushing deployment configuration to server...");

        var composeFile = await GetComposeFile(application.Repository, tagName);
        var customerName = customer.Name.Kebaberize();
        var containerNamePrefix =  $"{customerName}-{application.Name.Kebaberize()}";
        var remoteDeploymentPath = $"{customerName}/{application.Name.Kebaberize()}";

        var pushResult = await server.PushDeploymentConfig(application.Server, composeFile, remoteDeploymentPath);

        if (pushResult.IsFailure)
            throw new Exception($"Could not push deployment configuration to server with id = {application.Server.Id}. Error = {pushResult.Error}");

        logger.LogInformation("Deployment configuration pushed");

        logger.LogInformation("Promoting application...");

        var promoteResult = await server.Promote(application.Server, containerNamePrefix, pushResult.Value);

        if (promoteResult.IsFailure)
            throw new Exception($"Could not promote application on server with id = {application.Server.Id}. Error = {promoteResult.Error}");

        logger.LogInformation("Application promoted");
    }

    private async Task<string> GetComposeFile(string repository, string tagName)
    {
        var composeFile = await deploymentConfigFetcher.GetComposeFileAsync(repository, tagName);
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
