using DeploymentService.Domain;

namespace DeploymentService.Infrastructure;

public interface IDatabase
{
    public List<Customer> GetCustomers();
}