using Api.Domain;
using CSharpFunctionalExtensions;

namespace Api.Application;

public interface IServerConnection
{
    Task<Result<ServerInterrogationInfo>> InterrogateServer(Server server);
    Task<Result> PullDockerImage(Server server, DockerImage image);
    Task DockerPullAndRunAndAllThatStuff(Server server, string remoteDir);
}