namespace Api.Application;

public interface IApplicationRepository
{
    Task<List<Domain.Application>> GetApplications();
    Task<Domain.Application?> GetApplication(int id);
}