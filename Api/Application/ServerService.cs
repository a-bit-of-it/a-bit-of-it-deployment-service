using Api.Application.Interfaces;
using Api.Domain;
using Api.Exceptions;

namespace Api.Application;

public class ServerService (IServerConnection serverConnection, ICustomerRepository customerRepository)
{
    public async Task<List<ServerInterrogationInfo>> GetServers()
    {
        var servers = (await customerRepository.GetCustomers()).SelectMany(r => r.Applications).Select(r => r.Server).DistinctBy(s => s.Id).ToList();
        var serverStatuses = new List<ServerInterrogationInfo>();

        foreach (var server in servers)
        {
            var serverStatus = await serverConnection.InterrogateServer(server);

            if (serverStatus.IsFailure)
                serverStatuses.Add(new ServerInterrogationInfo() {ServerId =  server.Id, IsOnline = false});

            serverStatuses.Add(serverStatus.Value);
        }

        return serverStatuses;
    }

    public async Task<Server> GetServer(int id)
    {
        var server = (await customerRepository.GetCustomers())
            .SelectMany(customer => customer.Applications)
            .Select(application => application.Server)
            .DistinctBy(s => s.Id)
            .FirstOrDefault(s => s.Id == id);

        if (server == null)
            throw new NotFoundException($"Could not find server. Id = {id}.");

        return server;
    }

    public async Task<ServerComponents> GetComponents(int id)
    {
        var server = await GetServer(id);

        var componentsResult = await serverConnection.GetComponents(server);
        var allComponents = componentsResult.IsFailure ? [] : componentsResult.Value;

        var customers = await customerRepository.GetCustomers();
        var claimedContainerNames = new HashSet<string>();
        var customerGroups = new List<CustomerComponents>();

        foreach (var customer in customers)
        {
            var applicationsOnServer = customer.Applications.Where(app => app.Server.Id == id).ToList();

            if (applicationsOnServer.Count == 0)
                continue;

            var applicationGroups = new List<ApplicationComponents>();

            foreach (var application in applicationsOnServer)
            {
                var containerNamePrefix = ComponentNaming.GetContainerNamePrefix(customer.Name, application.Name);

                var matchedComponents = allComponents
                    .Where(component => component.ContainerName.StartsWith(containerNamePrefix))
                    .Select(component => component with
                    {
                        Name = ComponentNaming.GetShortComponentName(component.ContainerName, containerNamePrefix)
                    })
                    .ToList();

                foreach (var component in matchedComponents)
                    claimedContainerNames.Add(component.ContainerName);

                applicationGroups.Add(new ApplicationComponents(application.Id, application.Name, matchedComponents));
            }

            customerGroups.Add(new CustomerComponents(customer.Id, customer.Name, applicationGroups));
        }

        var unassigned = allComponents
            .Where(component => !claimedContainerNames.Contains(component.ContainerName))
            .ToList();

        return new ServerComponents(customerGroups, unassigned);
    }
}