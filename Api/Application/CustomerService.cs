using Api.Application.Interfaces;
using Api.Domain;
using Api.Exceptions;

namespace Api.Application;

public class CustomerService (ICustomerRepository customerRepository)
{
    public async Task<List<Customer>> GetAll()
    {
        return await customerRepository.GetCustomers();
    }

    public async Task<Customer> Get(int id)
    {
        var application = await customerRepository.GetCustomer(id);
        
        if  (application is null)
            throw new NotFoundException("No customer found.");
        
        return application;
    }
}