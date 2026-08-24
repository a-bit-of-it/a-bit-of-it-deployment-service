using Api.Domain;

namespace Api.Application.Interfaces;

public interface ICustomerRepository
{
    public Task<List<Customer>> GetCustomers();
    public Task<Customer?> GetCustomer(int id);
    Task<Customer?> GetCustomerByApplicationId(int applicationId);
}