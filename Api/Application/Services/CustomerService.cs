using Api.Domain;

namespace Api.Application.Services;

public class CustomerService (ICustomerRepository customerRepository)
{
    public async Task<List<Customer>> GetAllCustomers()
    {
        return await customerRepository.GetCustomers();
    }
}