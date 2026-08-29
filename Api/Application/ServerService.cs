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

    public async Task<List<Component>> GetComponents(int id)
    {
        var server = await GetServer(id);

        var componentsResult = await serverConnection.GetComponents(server);

        return componentsResult.IsFailure ? [] : componentsResult.Value;
    }
}