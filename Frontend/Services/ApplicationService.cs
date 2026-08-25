using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IApplicationService
{
    Task<Application?> GetAsync(int id, CancellationToken ct = default);
    Task<List<Tag>> GetTags(int id, CancellationToken ct = default);
    Task ReleaseAsync(int applicationId, Tag selectedTag);
    Task CreateTag(int id, CancellationToken ct = default);
    Task<Workflow?> GetWorkflow(int id, string commitSha, CancellationToken ct = default);
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

    public async Task CreateTag(int id, CancellationToken ct = default)
    {
        await http.PostAsJsonAsync($"api/applications/{id}/tags", new { }, ct);
    }
}