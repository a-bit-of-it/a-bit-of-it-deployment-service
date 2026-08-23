using System.Net;
using Api.Application;
using Api.Domain;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubImageRepository(HttpClient client) : IImageRepository
{
    private const string Registry = "ghcr.io";
    private const string Organization = Config.Organization;
    
    public async Task<List<DockerImage>> GetDockerImages(string repository)
    {
        var response = await client
            .GetAsync($"orgs/{Organization}/packages/container/{repository}/versions");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
            return new List<DockerImage>();
        
        var images =  await response.Content.ReadFromJsonAsync<List<GithubDockerImage>>() ?? new List<GithubDockerImage>();

        var imageBase = $"{Registry}/{Organization}/{repository}";
        
        return images
            .OrderByDescending(image => image.UpdatedAt)
            .Select(image => new DockerImage(image.Id, $"{imageBase}@{image.Name}", image.UpdatedAt))
            .ToList();
    }

    public async Task<DockerImage?> GetDockerImage(string repository, long imageId)
    {
        var image = await client
            .GetFromJsonAsync<GithubDockerImage>($"orgs/{Organization}/packages/container/{repository}/versions/{imageId}");

        if (image == null)
            return null;
        
        var imageBase = $"{Registry}/{Organization}/{repository}";

        return new DockerImage(image.Id, $"{imageBase}@{image.Name}", image.UpdatedAt);
    }
}