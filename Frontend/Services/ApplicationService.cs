using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface IApplicationService
{
    public Task<Application?> GetAsync(int id, CancellationToken ct = default);
}

public class ApplicationService(HttpClient http) : IApplicationService
{
    public async Task<Application?> GetAsync(int id, CancellationToken ct = default)
    {
        var application = await http.GetFromJsonAsync<Application>($"api/applications/{id}", ct);
        return application;
    }
    
}