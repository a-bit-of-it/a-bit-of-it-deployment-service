using System.Net.Http.Json;
using FrontendWasm.Models;

namespace FrontendWasm.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetCustomersAsync(CancellationToken ct = default);
}

public class CustomerService : ICustomerService
{
    private readonly HttpClient _http;

    public CustomerService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Customer>> GetCustomersAsync(CancellationToken ct = default)
    {
        var customers = await _http.GetFromJsonAsync<List<Customer>>("api/customers", ct);
        return customers ?? [];
    }
}