namespace DeploymentService.Application.Services;

public class DeploymentService (ICustomerRepository customerRepository, IImageRepository imageRepository, IServerConnection server)
{
    public async Task Deploy(int customerId, int applicationId, long imageId)
    {
        var customer = await customerRepository.GetCustomer(customerId);
        
        if (customer == null)
            throw new Exception("Customer not found.");
        
        var app = customer.Applications.FirstOrDefault(app => app.Id == applicationId);
        
        if (app == null)
            throw new Exception("Application not found.");
        
        var image = await imageRepository.GetDockerImage(app.Registry, imageId);
        
        if (image == null)
            throw new Exception("Image not found.");

        await server.PullDockerImage(app.Server, image);
    }
}