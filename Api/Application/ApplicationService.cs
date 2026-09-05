using Api.Application.Interfaces;
using Api.Application.Models;
using Api.Domain;
using Api.Exceptions;

namespace Api.Application;

public class ApplicationService (ICustomerRepository customerRepository, IApplicationRepository applicationRepository, IServerConnection server, ITagRepository tagRepository, IWorkflowRepository workflowRepository, IReleaseRepository releaseRepository, ILogger<ApplicationService> logger)
{
    public async Task<List<Tag>> GetTags(int id)
    {
        var application = await applicationRepository.GetApplication(id);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var tags = await tagRepository.GetTags(application.Repository);

        return tags;
    }
    
    public async Task<Tag> CreateTag(int id)
    {
        logger.LogInformation("Creating tag...");
        var application = await applicationRepository.GetApplication(id);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var tag = await tagRepository.CreateTag(application.Repository);
        
        logger.LogInformation("Tag {TagName} created for {ApplicationName}", tag.Name, application.Name);

        return tag;
    }
    
    public async Task<Workflow> GetWorkflow(int id, Tag tag)
    {
        var application = await applicationRepository.GetApplication(id);
        
        if (application is null)
            throw new NotFoundException("No application found.");
        
        var workflow = await workflowRepository.GetWorkflow(application.Repository, tag);
        
        if  (workflow == null)
            throw new NotFoundException($"Workflow for tag {tag.Name} not found.");
        
        return workflow;
    }
    
    public async Task<ApplicationContainers> GetContainers(int applicationId)
    {
        var customer = await customerRepository.GetCustomerByApplicationId(applicationId);

        if (customer == null)
            throw new NotFoundException($"Could not find customer from application id. Id = {applicationId}.");

        var application = customer.Applications.FirstOrDefault(app => app.Id == applicationId);

        if (application == null)
            throw new NotFoundException($"Could not find application. Id = {applicationId}.");

        var result = await server.GetContainers(application.Server);
        
        if (result.IsFailure)
            throw new Exception($"Could not get containers on server with id {application.Server.Id}. Error = {result.Error}");

        var containerNamePrefix = ComponentNaming.GetContainerNamePrefix(customer.Name, application.Name);

        var applicationContainers = result.Value
            .Where(component => component.ContainerName.StartsWith(containerNamePrefix));
        
        var containers = applicationContainers
            .Select(container => new ApplicationContainer(ComponentNaming.GetShortComponentName(container.Name, containerNamePrefix), 
                container.ContainerName, 
                container.Image, 
                container.IsRunning, 
                container.Status, 
                container.Ports)
            ).ToList();
        
        return new ApplicationContainers(applicationId, application.Name, containers);
    }

    public async Task<List<Domain.Application>> GetAll()
    {
        return await applicationRepository.GetApplications();
    }
    
    public async Task<Domain.Application> Get(int id)
    {
        var application = await applicationRepository.GetApplication(id);

        if (application is null)
            throw new NotFoundException("No application found.");

        return application;
    }

    public async Task<Release> GetLatestRelease(int id)
    {
        var application = await applicationRepository.GetApplication(id);

        if (application is null)
            throw new NotFoundException("No application found.");

        var release = await releaseRepository.GetLatestRelease(application.Repository);
        
        if (release == null)
            throw new NotFoundException("No latest release.");
        
        return release;
    }

    public async Task<Release> GetRelease(int id, string tagName)
    {
        var application = await applicationRepository.GetApplication(id);

        if (application is null)
            throw new NotFoundException("No application found.");

        var release = await releaseRepository.GetRelease(application.Repository, tagName);
        
        if (release == null)
            throw new NotFoundException("Could not find release for tag {tagName}.");
        
        return  release;
    }
}