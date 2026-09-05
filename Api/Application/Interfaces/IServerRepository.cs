namespace Api.Application.Interfaces;

public interface IServerRepository
{
    Task<List<Domain.Server>> GetServers();
    Task<Domain.Server?> GetServer(int id);
}