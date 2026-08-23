using DeploymentService.Exceptions;

namespace DeploymentService.Application.Services;

public class DeploymentService (ICustomerRepository customerRepository, IImageRepository imageRepository, IServerConnection server, ILogger<DeploymentService> logger)
{
    public async Task Deploy(int customerId, int applicationId, long imageId)
    {
        logger.LogInformation("Beginning deployment...");
        var customer = await customerRepository.GetCustomer(customerId);

        if (customer == null)
            throw new NotFoundException($"Could not find customer. Id = {customerId}.");
        
        var app = customer.Applications.FirstOrDefault(app => app.Id == applicationId);
        
        if (app == null)
            throw new NotFoundException($"Could not find application. Id = {applicationId}.");
        
        var image = await imageRepository.GetDockerImage(app.Registry, imageId);
        
        if (image == null)
            throw new NotFoundException($"Could not find image. Id = {imageId}.");
        
        logger.LogInformation("Deploying {ImageImage} to {CustomerName} on {ServerIp}...", image.Image, customer.Name, app.Server.Ip);

        var response = await server.PullDockerImage(app.Server, image);
        
        if (response.IsFailure)
            throw new DeploymentException($"Could not complete deployment. Ip = {app.Server.Ip}. Error = {response.Error}.");
        
        logger.LogInformation("Deployment complete.");
    }
}