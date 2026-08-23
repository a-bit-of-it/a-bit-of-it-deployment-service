using Api.Application;
using Api.Domain;

namespace Api.Infrastructure.Database;

// Temp stuff
public class CustomerRepository : ICustomerRepository
{
    List<Customer> users;
    public CustomerRepository()
    {
        var server = new Domain.Server(0, "85.190.97.44");
        
        var deploymentService = new Domain.Application(0, "Deployment Service", "deployment-service",
            "https://github.com/a-bit-of-it/deployment-service", server);
        var website = new Domain.Application(1, "Website", "website",
            "https://github.com/a-bit-of-it/website", server);
        
        var iAmACustomerMyselfHehe = new Customer(0, "a-bit-of-it", "Right here", "Denmark", "Aalborg", "123456789",
            new List<Domain.Application>(){website, deploymentService});
        
        var firstCustomerApplication = new Domain.Application(3, "Website", "gastronomia-napolitana-website", "https://github.com/a-bit-of-it/gastronomia-napolitana", server);
        var firstCustomer = new Customer(1, "Gastronomia Napolitana", "John F. Kennedys Pl. 2", "Denmark", "Aalborg",
            "98122911", new List<Domain.Application>(){firstCustomerApplication});

        // Would a dream this is, huh?
        var secondCustomer = new Customer(2, "LEGO", "hihi plads", "haha country", "haha city", "hehe phonenumber",
            new List<Domain.Application>());

        users = new List<Customer>() { iAmACustomerMyselfHehe, firstCustomer, secondCustomer };
    }
    
    public Task<List<Customer>> GetCustomers()
    {
        return Task.FromResult(users);
    }

    public Task<Customer?> GetCustomer(int id)
    {
        return Task.FromResult(users.FirstOrDefault(x => x.Id == id));
    }
}