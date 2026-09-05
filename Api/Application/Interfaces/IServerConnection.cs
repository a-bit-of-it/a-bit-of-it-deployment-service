using Api.Domain;
using CSharpFunctionalExtensions;

namespace Api.Application.Interfaces;

public interface IServerConnection
{
    Task<Result<List<Container>>> GetContainers(Server server);
    Task<Result<string>> PushDeploymentConfig(Server server, string contents, string folder);
    Task<Result> Promote(Server server, string containerNamePrefix, string remoteDir);
}