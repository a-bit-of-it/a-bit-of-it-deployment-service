using System.Net;
using Api.Application.Interfaces;
using Api.Domain;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubReleaseRepository(HttpClient client, ILogger<GithubWorkflowRepository> logger) : IReleaseRepository
{
    public async Task<Release?> GetRelease(string repository, string tagName)
    {
        var response = await client.GetAsync(
            $"repos/{Config.Organization}/{repository}/releases/tags/{tagName}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var release = await response.Content.ReadFromJsonAsync<GithubRelease>()
                      ?? throw new InvalidOperationException(
                          $"Could not parse release response for {repository}@{tagName}");

        return new Release(release.Id, release.Name, release.CreatedAt, release.Url);
    }
    
    public async Task<Release?> GetLatestRelease(string repository)
    {
        var response = await client.GetAsync(
            $"repos/{Config.Organization}/{repository}/releases/latest");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        var release = await response.Content.ReadFromJsonAsync<GithubRelease>()
                      ?? throw new InvalidOperationException(
                          $"Could not parse latest release response for {repository}");

        return new Release(release.Id, release.Name, release.CreatedAt, release.Url);
    }

    public async Task SetLatestRelease(string repository, long releaseId)
    {
        var response = await client.PatchAsJsonAsync(
            $"repos/{Config.Organization}/{repository}/releases/{releaseId}",
            new { make_latest = "true" });

        response.EnsureSuccessStatusCode();
    }

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

        return new Release(release.Id, release.Name, release.CreatedAt, release.Url);
    }
}