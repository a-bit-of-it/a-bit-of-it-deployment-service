using Api.Domain;
using CSharpFunctionalExtensions;

namespace Api.Application;

public interface IServerConnection
{
    public Task<Result<ServerInterrogationInfo>> InterrogateServer(Server server);
    public Task<Result> PullDockerImage(Server server, DockerImage image);
}