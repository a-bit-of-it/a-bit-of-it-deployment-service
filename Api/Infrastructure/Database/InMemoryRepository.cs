using Api.Application;
using Api.Application.Interfaces;
using Api.Domain;

namespace Api.Infrastructure.Database;

// Temp stuff
public class InMemoryRepository : ICustomerRepository, IApplicationRepository, IServerRepository
{
    private readonly List<Customer> _users;
    
    public InMemoryRepository()
    {
        var server = new Domain.Server(0, "85.190.97.44");
        
        var deploymentService = new Domain.Application(0, "Deployment Service", "a-bit-of-it-deployment-service",
            "https://github.com/a-bit-of-it/deployment-service", server);
        
        var website = new Domain.Application(1, "Website", "a-bit-of-it-website",
            "https://github.com/a-bit-of-it/website", server);
        
        var iAmACustomerMyselfHehe = new Customer(0, "a-bit-of-it", "Right here", "Denmark", "Aalborg", "123456789",
            new List<Domain.Application>(){website, deploymentService});
        
        
        var firstCustomerApplication = new Domain.Application(3, "Website", "gastronomia-napolitana-website", "https://github.com/a-bit-of-it/gastronomia-napolitana", server);
        
        var firstCustomer = new Customer(1, "Gastronomia Napolitana", "John F. Kennedys Pl. 2", "Denmark", "Aalborg",
            "98122911", new List<Domain.Application>(){firstCustomerApplication});


        var secondCustomerApplication = new Domain.Application(4, "Website", "koreskole-website",
            "https://github.com/a-bit-of-it/koreskole-website", server);
        
        var secondCustomer = new Customer(2, "Koreskole", "Koreskoke.dk", "Denmark", "Dunno", "Also dunno",
            new List<Domain.Application>(){secondCustomerApplication});

        _users = new List<Customer>() { iAmACustomerMyselfHehe, firstCustomer, secondCustomer };
    }
    
    public Task<List<Customer>> GetCustomers()
    {
        return Task.FromResult(_users);
    }

    public Task<Customer?> GetCustomer(int id)
    {
        return Task.FromResult(_users.FirstOrDefault(x => x.Id == id));
    }

    public Task<Customer?> GetCustomerByApplicationId(int applicationId)
    {
        return Task.FromResult(_users.FirstOrDefault(u => u.Applications.Any(a => a.Id == applicationId)));
    }

    public Task<List<Customer>> GetCustomersByServerId(int serverId)
    {
        return Task.FromResult(_users.Where(user => user.Applications.Any(r => r.Server.Id == serverId)).ToList());
    }

    public Task<List<Domain.Application>> GetApplications()
    {
        return Task.FromResult(_users.SelectMany(x => x.Applications).ToList());
    }

    public Task<Domain.Application?> GetApplication(int id)
    {
        return Task.FromResult(_users.SelectMany(x => x.Applications).FirstOrDefault(app => app.Id == id));
    }

    public Task<List<Domain.Server>> GetServers()
    {
        return Task.FromResult(_users.SelectMany(customer => customer.Applications)
            .Select(application => application.Server)
            .DistinctBy(s => s.Id)
            .ToList());
    }

    public Task<Domain.Server?> GetServer(int id)
    {
        return Task.FromResult(_users.SelectMany(customer => customer.Applications)
            .Select(application => application.Server)
            .DistinctBy(s => s.Id)
            .FirstOrDefault(server => server.Id == id));
    }
}