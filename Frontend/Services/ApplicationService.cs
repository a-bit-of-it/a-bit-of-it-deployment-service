using System.Net;
using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IApplicationService
{
    Task<Application?> GetAsync(int id, CancellationToken ct = default);
    Task<List<Tag>> GetTags(int id, CancellationToken ct = default);
    Task ReleaseAsync(int applicationId, Tag selectedTag);
    Task<Tag?> CreateTag(int id, CancellationToken ct = default);
    Task<Workflow?> GetWorkflow(int id, string tag, CancellationToken ct = default);
    Task<Release?> GetLatestRelease(int id, CancellationToken ct = default);
    Task<Release?> GetRelease(int id, string tagName, CancellationToken ct = default);
    Task Rollback(int applicationId, Tag tag);
    Task<List<ContainerInfo>> GetContainers(int id, CancellationToken ct = default);
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
    
    public Task<Workflow?> GetWorkflow(int id, string tag, CancellationToken ct = default)
    {
       return GetOrDefaultAsync<Workflow>($"api/applications/{id}/workflows/{tag}", ct);
    }

    public async Task ReleaseAsync(int id, Tag tag)
    {
        var body = new { tag };
        await http.PostAsJsonAsync($"api/applications/{id}/deployments", body);
    }

    public Task<Release?> GetLatestRelease(int id, CancellationToken ct = default)
    {
        return GetOrDefaultAsync<Release>($"api/applications/{id}/releases/latest", ct);
    }

    public Task<Release?> GetRelease(int id, string tagName, CancellationToken ct = default)
    {
        return GetOrDefaultAsync<Release>($"api/applications/{id}/releases/{tagName}", ct);
    }

    private async Task<T?> GetOrDefaultAsync<T>(string requestUri, CancellationToken ct)
    {
        var response = await http.GetAsync(requestUri, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return default;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: ct);
    }

    public async Task Rollback(int id, Tag tag)
    {
        var body = new { tag };
        await http.PostAsJsonAsync($"api/applications/{id}/rollbacks", body);
    }

    public async Task<List<ContainerInfo>> GetContainers(int id, CancellationToken ct = default)
    {
        var containers = await http.GetFromJsonAsync<List<ContainerInfo>>($"api/applications/{id}/containers", ct) ?? [];
        return containers;
    }

    public async Task<Tag?> CreateTag(int id, CancellationToken ct = default)
    {
        var response = await http.PostAsJsonAsync($"api/applications/{id}/tags", new { }, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadFromJsonAsync<Tag>(cancellationToken: ct) ?? throw new Exception("Could not parse response.");
        
        var error = await response.Content.ReadAsStringAsync(ct);
        throw new Exception("Could not create tag " + error);
    }
}