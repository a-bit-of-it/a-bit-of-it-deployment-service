using Api.Domain;

namespace Api.Application.Services;

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
}