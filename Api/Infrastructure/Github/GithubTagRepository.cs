using System.Net;
using Api.Domain;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubTagRepository(HttpClient client)// : IImageRepository
{
    private const string Organization = Config.Organization;
    
    public async Task<List<Tag>> GetTags(string repository)
    {
        var response = await client
            .GetAsync($"repos/{Organization}/{repository}/tags");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return new List<Tag>();

        var tags = await response.Content.ReadFromJsonAsync<List<GithubTag>>()
                   ?? new List<GithubTag>();

        return tags
            .Select(tag => new Tag(tag.Name, tag.Commit.Sha))
            .ToList();
    }
}