using Api.Domain;
using CSharpFunctionalExtensions;

namespace Api.Application.Interfaces;

public interface IServerConnection
{
    Task<Result<ServerInterrogationInfo>> InterrogateServer(Server server);
    Task<Result> PullDockerImage(Server server, DockerImage image);
    Task Deploy(Server server, string containerNamePrefix, string remoteDir);
}