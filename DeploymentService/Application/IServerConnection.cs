using DeploymentService.Domain;

namespace DeploymentService.Application;

public interface IServerConnection
{
    public Task PullDockerImage(Server server, DockerImage image);
}