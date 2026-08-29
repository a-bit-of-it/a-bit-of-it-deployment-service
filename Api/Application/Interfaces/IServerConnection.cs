using Api.Domain;
using CSharpFunctionalExtensions;

namespace Api.Application.Interfaces;

public interface IServerConnection
{
    Task<Result<ServerInterrogationInfo>> InterrogateServer(Server server);
    Task<Result<List<Component>>> GetComponents(Server server);
    Task<Result> Deploy(Server server, string containerNamePrefix, string remoteDir);
}