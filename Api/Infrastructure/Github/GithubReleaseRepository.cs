using Api.Application.Interfaces;
using Api.Domain;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubReleaseRepository(HttpClient client, ILogger<GithubWorkflowRepository> logger) : IReleaseRepository
{
    public async Task<Release> CreateRelease(string repository, string tagName)
    {
        var response = await client.PostAsJsonAsync(
            $"repos/{Config.Organization}/{repository}/releases",
            new
            {
                tag_name = tagName,
                name = tagName,
                generate_release_notes = true
            });

        response.EnsureSuccessStatusCode();

        var release = await response.Content.ReadFromJsonAsync<GithubRelease>()
                      ?? throw new InvalidOperationException(
                          $"Could not parse release response for {repository}@{tagName}");

        return new Release(release.Id, release.Name, release.HtmlUrl);
    }
}