using DeploymentService.Application;
using DeploymentService.Controllers.Responses;
using DeploymentService.Domain;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentService.Controllers;

[Route("api/[controller]")]
public class DeploymentsController(Application.Services.DeploymentService deploymentService, ICustomerRepository customerRepository, IImageRepository imageRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAvailableDeployments()
    {
        var customers = await customerRepository.GetCustomers();
        List<CustomerResponse> customerResponses = new List<CustomerResponse>();
        foreach (var customer in customers)
        {
            var appResponses = await GetApplicationResponse(customer);

            customerResponses.Add(new CustomerResponse(customer.Id, appResponses));
        }
        
        return Ok(customerResponses);
    }
    
    private async Task<List<ApplicationResponse>> GetApplicationResponse(Customer customer)
    {
        List<ApplicationResponse> applications = new List<ApplicationResponse>();
        foreach (var application in customer.Applications)
        {
            var dockerImages = await imageRepository.GetDockerImages(application.Registry);
            var appResponse = new ApplicationResponse(application.Id, dockerImages);    
            applications.Add(appResponse);
        }

        return applications;
    }
    
    
    public class DeploymentRequest
    {
        public required int CustomerId { get; init; }
        public required int ApplicationId { get; init; }
        public required long ImageId { get; set; }
    }
    
    [HttpPost]
    public async Task<IActionResult> Deploy([FromBody] DeploymentRequest request)
    {
        await deploymentService.Deploy(
            request.CustomerId,
            request.ApplicationId,
            request.ImageId);

        return Ok();
    }
}