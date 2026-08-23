using System.Text.RegularExpressions;
using Api.Exceptions;
using Api.Infrastructure.Github;

namespace Api.Application.Services;

public class DeploymentService (ICustomerRepository customerRepository, IImageRepository imageRepository, IServerConnection server, GithubFileFetcher githubFileFetcher, ILogger<DeploymentService> logger)
{
    public async Task Deploy(int customerId, int applicationId)
    {
        logger.LogInformation("Beginning deployment...");
        var customer = await customerRepository.GetCustomer(customerId);

        if (customer == null)
            throw new NotFoundException($"Could not find customer. Id = {customerId}.");
        
        var app = customer.Applications.FirstOrDefault(app => app.Id == applicationId);
        
        if (app == null)
            throw new NotFoundException($"Could not find application. Id = {applicationId}.");
        
        var hardcodedTagTemp = "v2026-08-23-r2";

        var composeFile = await githubFileFetcher.GetComposeFileAsync(app.Repo, hardcodedTagTemp);
        var composeFileResultWhatever = ResolveAndValidateCompose(composeFile, app.Repo, hardcodedTagTemp);

        Console.WriteLine(composeFileResultWhatever);
    }
    
    private static readonly Regex ImagePlaceholderPattern = new(@"\$\{(\w+)\}", RegexOptions.Compiled);

    public ComposeValidationResult ResolveAndValidateCompose(
        string composeYaml, string repoName, string tag)
    {
        var expectedComponents = ImagePlaceholderPattern
            .Matches(composeYaml)
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();

        if (expectedComponents.Count == 0)
            throw new InvalidOperationException($"No image placeholders found in compose file for '{repoName}'.");

        var imageRefs = expectedComponents.ToDictionary(
            component => component,
            component => $"ghcr.io/a-bit-of-it/{repoName}-{component}:{tag}");

        var resolvedYaml = composeYaml;
        foreach (var (component, imageRef) in imageRefs)
            resolvedYaml = resolvedYaml.Replace($"${{{component}}}", imageRef);

        if (resolvedYaml.Contains("${"))
            throw new InvalidOperationException($"Unresolved placeholder remains in compose file for '{repoName}'.");

        return new ComposeValidationResult
        {
            ResolvedComposeYaml = resolvedYaml,
            ComponentImageRefs = imageRefs
        };
    }
    
    public class ComposeValidationResult
    {
        public required string ResolvedComposeYaml { get; init; }
        public required Dictionary<string, string> ComponentImageRefs { get; init; }
    }
}