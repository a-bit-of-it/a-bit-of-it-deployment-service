using Api.Application.Interfaces;
using Api.Application.Models;
using Api.Domain;
using Api.Exceptions;

namespace Api.Application;

public class ServerService (IServerConnection serverConnection, ICustomerRepository customerRepository, IServerRepository serverRepository)
{
    public async Task<List<Server>> GetServers()
    {
        var servers = await serverRepository.GetServers();

        return servers;
    }
    
    public async Task<Server> GetServer(int id)
    {
        var server = await serverRepository.GetServer(id);
        
        if (server == null)
            throw new NotFoundException($"Server with id {id} not found");
        
        return server;
    }
    
    public async Task<ServerStatus> GetServerStatus(int id)
    {
        var server = await serverRepository.GetServer(id);
        
        if (server == null)
            throw new NotFoundException($"Server with id {id} not found");
        
        var containers = await serverConnection.GetContainers(server);

        if (containers.IsFailure)
            return new ServerStatus(server.Id, false, containers.Error);

        return new ServerStatus(server.Id, true, string.Empty);
    }
    
    public async Task<FleetStatus> GetFleetStatus()
    {
        var servers = await serverRepository.GetServers();
        var statuses = new List<ServerStatus>();

        foreach (var server in servers)
        {
            var containers = await serverConnection.GetContainers(server);

            if (containers.IsFailure)
                statuses.Add(new ServerStatus(server.Id, false, containers.Error));

            statuses.Add(new ServerStatus(server.Id, true, string.Empty));
        }
        
        var isAllOnline = statuses.All(status => status.IsOnline);

        return new FleetStatus(isAllOnline, statuses);
    }

    public async Task<ServerContainers> GetContainers(int id)
    {
        var server = await serverRepository.GetServer(id);
        
        if  (server == null)
            throw new NotFoundException($"Server with id {id} not found");

        var result = await serverConnection.GetContainers(server);
        
        if (result.IsFailure)
            throw new Exception($"Could not get containers on server with id {id}. Error = {result.Error}");
        
        var customers = await customerRepository.GetCustomersByServerId(id);
        var customerContainers = new List<CustomerContainers>();

        foreach (var customer in customers)
        {
            var applications = customer.Applications;
            var applicationGroups = new List<ApplicationContainers>();

            foreach (var application in applications)
            {
                var containerNamePrefix = ComponentNaming.GetContainerNamePrefix(customer.Name, application.Name);

                var applicationContainers = result.Value
                    .Where(component => component.ContainerName.StartsWith(containerNamePrefix));
                
                var matchedComponents = applicationContainers
                    .Select(container => new ApplicationContainer(ComponentNaming.GetShortComponentName(container.Name, containerNamePrefix), 
                        container.ContainerName, 
                        container.Image, 
                        container.IsRunning, 
                        container.Status, 
                        container.Ports)
                    ).ToList();
                
                applicationGroups.Add(new ApplicationContainers(application.Id, application.Name, matchedComponents));
            }

            customerContainers.Add(new CustomerContainers(customer.Id, customer.Name, applicationGroups));
        }

        return new ServerContainers(customerContainers);
    }
}