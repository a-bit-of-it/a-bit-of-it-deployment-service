using System.Text.RegularExpressions;
using Api.Domain;
using Api.Exceptions;
using Api.Infrastructure.Github;

namespace Api.Application.Services;

public class ApplicationService (ICustomerRepository customerRepository, IApplicationRepository applicationRepository, IServerConnection server, IFilePusher filePusher, GithubFileFetcher githubFileFetcher, ITagRepository tagRepository, ILogger<ApplicationService> logger)
{
    public async Task Deploy(int applicationId, string tag)
    {
        logger.LogInformation("Beginning deployment...");
        var customer = await customerRepository.GetCustomerByApplicationId(applicationId);

        if (customer == null)
            throw new NotFoundException($"Could not find customer from application id. Id = {applicationId}.");
        
        var application = customer.Applications.FirstOrDefault(app => app.Id == applicationId);
        
        if (application == null)
            throw new NotFoundException($"Could not find application. Id = {applicationId}.");
        
        var composeFile = await GetComposeFile(application.Repository, tag);
        
        var remoteDir = await filePusher.Push(application.Server, composeFile, $"{customer.Name}-{application.Repository}");

        await server.DockerPullAndRunAndAllThatStuff(application.Server, remoteDir);

        logger.LogInformation("Deploy succeeded for {ApplicationName}", application.Name);
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

    public async Task<List<Tag>> GetTags(int applicationId)
    {
        var application = await applicationRepository.GetApplication(applicationId);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var tags = await tagRepository.GetTags(application.Repository);

        return tags;
    }
    
    private async Task<string> GetComposeFile(string repository, string hardcodedTagTemp)
    {
        var composeFile = await githubFileFetcher.GetComposeFileAsync(repository, hardcodedTagTemp);
        var resolvedComposeFile = ResolveComposeFile(composeFile, repository, hardcodedTagTemp);

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
            component => $"ghcr.io/a-bit-of-it/{repository}-{component}:{tag}");

        var resolvedYaml = composeYaml;
        foreach (var (component, imageRef) in imageRefs)
            resolvedYaml = resolvedYaml.Replace($"${{{component}}}", imageRef);

        if (resolvedYaml.Contains("${"))
            throw new InvalidOperationException($"Unresolved placeholder remains in compose file for '{repository}'.");

        return resolvedYaml;
    }
}