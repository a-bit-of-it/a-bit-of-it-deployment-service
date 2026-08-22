using DeploymentService.Domain;

namespace DeploymentService.Application;

public interface ICustomerRepository
{
    public Task<List<Customer>> GetCustomers();
    public Task<Customer?> GetCustomer(int id);
}