using CSharpFunctionalExtensions;
using DeploymentService.Domain;

namespace DeploymentService.Application;

public interface IServerConnection
{
    public Task<Result<ServerInterrogationInfo>> InterrogateServer(Server server);
    public Task<Result> PullDockerImage(Server server, DockerImage image);
}