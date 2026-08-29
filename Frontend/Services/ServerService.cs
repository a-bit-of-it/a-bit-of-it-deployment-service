using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IServerService
{
    Task<FleetStatusResponse?> GetFleetStatus(CancellationToken ct = default);
}

public class ServerService(HttpClient http) : IServerService
{
    public async Task<FleetStatusResponse?> GetFleetStatus(CancellationToken ct = default)
    {
        var status = await http.GetFromJsonAsync<FleetStatusResponse>("api/servers/fleet-status", ct);
        return status;
    }
}