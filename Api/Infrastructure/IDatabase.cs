using Api.Domain;

namespace Api.Infrastructure;

public interface IDatabase
{
    public List<Customer> GetCustomers();
}