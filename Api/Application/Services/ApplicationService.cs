using Api.Exceptions;

namespace Api.Application.Services;

public class ApplicationService (IApplicationRepository applicationRepository)
{
    public async Task<List<Domain.Application>> GetAll()
    {
        return await applicationRepository.GetApplications();
    }
    
    public async Task<Domain.Application> Get(int id)
    {
        var application = await applicationRepository.GetApplication(id);
        
        if  (application is null)
            throw new NotFoundException("No application found.");
        
        return application;
    }
}