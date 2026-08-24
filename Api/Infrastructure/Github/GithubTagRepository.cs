using System.Net;
using Api.Application;
using Api.Domain;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubTagRepository(HttpClient client, ILogger<GithubTagRepository> logger) : ITagRepository
{
    private const string Organization = Config.Organization;
    private const string MainBranch = "main";
    
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

    public async Task<Tag> CreateTag(string applicationRepository)
    {
        var sha = await GetLatestCommitSha(applicationRepository, MainBranch);
        var tagName = await GetNextTagName(applicationRepository);

        var response = await client.PostAsJsonAsync(
            $"repos/{Organization}/{applicationRepository}/git/refs",
            new
            {
                @ref = $"refs/tags/{tagName}",
                sha
            });

        response.EnsureSuccessStatusCode();

        return new Tag(tagName, sha);
    }

    private async Task<string> GetLatestCommitSha(string repository, string branch)
    {
        var response = await client
            .GetAsync($"repos/{Organization}/{repository}/commits/{branch}");

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            logger.LogError("Failed to fetch commit sha for {repository}/{branch}. Error = {error}", repository, branch, error);
            throw new Exception($"Failed to fetch commit sha for {repository}/{branch}");
        }
        
        var commit = await response.Content.ReadFromJsonAsync<GithubCommit>()
                     ?? throw new InvalidOperationException(
                         $"Could not resolve latest commit for {repository}@{branch}");

        return commit.Sha;
    }

    private async Task<string> GetNextTagName(string repository)
    {
        var today = DateTime.UtcNow.ToString("yyyy-MM-dd");
        var existingTags = await GetTags(repository);

        var todaysReleaseNumbers = existingTags
            .Select(t => t.Name)
            .Where(name => name.StartsWith($"v{today}-r"))
            .Select(name => int.TryParse(name.Split("-r").Last(), out var n) ? n : 0)
            .ToList();

        var nextRelease = todaysReleaseNumbers.Any()
            ? todaysReleaseNumbers.Max() + 1
            : 1;

        return $"v{today}-r{nextRelease}";
    }
}