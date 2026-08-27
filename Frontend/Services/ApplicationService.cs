using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IApplicationService
{
    Task<Application?> GetAsync(int id, CancellationToken ct = default);
    Task<List<Tag>> GetTags(int id, CancellationToken ct = default);
    Task ReleaseAsync(int applicationId, Tag selectedTag);
    Task<Tag> CreateTag(int id, CancellationToken ct = default);
    Task<Workflow?> GetWorkflow(int id, string commitSha, CancellationToken ct = default);
    Task<Release?> GetLatestRelease(int id, CancellationToken ct = default);
}

public class ApplicationService(HttpClient http) : IApplicationService
{
    public async Task<Application?> GetAsync(int id, CancellationToken ct = default)
    {
        var application = await http.GetFromJsonAsync<Application>($"api/applications/{id}", ct);
        return application;
    }

    public async Task<List<Tag>> GetTags(int id, CancellationToken ct = default)
    {
        var tags = await http.GetFromJsonAsync<List<Tag>>($"api/applications/{id}/tags", ct) ?? [];
        return tags;
    }
    
    public async Task<Workflow?> GetWorkflow(int id, string commitSha, CancellationToken ct = default)
    {
        var workflow = await http.GetFromJsonAsync<Workflow>($"api/applications/{id}/workflows/{commitSha}", ct);
        return workflow;
    }

    public async Task ReleaseAsync(int id, Tag tag)
    {
        var body = new { tag };
        await http.PostAsJsonAsync($"api/applications/{id}/deployments", body);
    }

    public async Task<Release?> GetLatestRelease(int id, CancellationToken ct = default)
    {
        var release = await http.GetFromJsonAsync<Release>($"api/applications/{id}/releases/latest", ct);
        return release;
    }

    public async Task<Tag> CreateTag(int id, CancellationToken ct = default)
    {
        var haah = await http.PostAsJsonAsync($"api/applications/{id}/tags", new { }, ct);

        if (haah.IsSuccessStatusCode)
            return await haah.Content.ReadFromJsonAsync<Tag>(cancellationToken: ct) ?? throw new Exception("Could not parse response.");
        
        var error = await haah.Content.ReadAsStringAsync(ct);
        throw new Exception("Could not create tag " + error);
    }
}