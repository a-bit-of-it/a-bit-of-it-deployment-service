using System.Net.Http.Json;
using Frontend.Models;

namespace Frontend.Services;

public interface ICustomerService
{
    Task<List<Customer>> GetAllAsync(CancellationToken ct = default);
    Task<Customer?> GetAsync(int customerId,  CancellationToken ct = default);
}

public class CustomerService : ICustomerService
{
    private readonly HttpClient _http;

    public CustomerService(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<Customer>> GetAllAsync(CancellationToken ct = default)
    {
        var customers = await _http.GetFromJsonAsync<List<Customer>>("api/customers", ct);
        return customers ?? [];
    }
    
    public async Task<Customer?> GetAsync(int id, CancellationToken ct = default)
    {
        var customer = await _http.GetFromJsonAsync<Customer>($"api/customers/{id}", ct);
        return customer;
    }
}