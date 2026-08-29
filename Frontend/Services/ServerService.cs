using System.Net;
using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IServerService
{
    Task<FleetStatusResponse?> GetFleetStatus(CancellationToken ct = default);
    Task<Server?> GetAsync(int id, CancellationToken ct = default);
    Task<List<ContainerInfo>> GetComponents(int id, CancellationToken ct = default);
}

public class ServerService(HttpClient http) : IServerService
{
    public async Task<FleetStatusResponse?> GetFleetStatus(CancellationToken ct = default)
    {
        var status = await http.GetFromJsonAsync<FleetStatusResponse>("api/servers/fleet-status", ct);
        return status;
    }

    public async Task<Server?> GetAsync(int id, CancellationToken ct = default)
    {
        var response = await http.GetAsync($"api/servers/{id}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<Server>(cancellationToken: ct);
    }

    public async Task<List<ContainerInfo>> GetComponents(int id, CancellationToken ct = default)
    {
        var components = await http.GetFromJsonAsync<List<ContainerInfo>>($"api/servers/{id}/components", ct) ?? [];
        return components;
    }
}