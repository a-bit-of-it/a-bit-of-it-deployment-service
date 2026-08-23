using Api.Domain;

namespace Api.Application;

public interface ICustomerRepository
{
    public Task<List<Customer>> GetCustomers();
    public Task<Customer?> GetCustomer(int id);
}