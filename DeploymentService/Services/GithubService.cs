using DeploymentService.Models;
using DeploymentService.Services.DTOs;

namespace DeploymentService.Services;

public class GithubService(HttpClient client) : IGithubService
{
    private const string Registry = "ghcr.io";
    private const string Organization = Config.Organization;
    
    public async Task<List<DockerImage>> GetDockerImages(string repository)
    {
        var images = await client
            .GetFromJsonAsync<List<GithubDockerImage>>($"orgs/{Organization}/packages/container/{repository}/versions") ?? [];

        var imageBase = $"{Registry}/{Organization}/{repository}";
        
        return images
            .OrderByDescending(image => image.UpdatedAt)
            .Select(image => new DockerImage($"{imageBase}@{image.Name}", image.UpdatedAt))
            .ToList();
    }
}