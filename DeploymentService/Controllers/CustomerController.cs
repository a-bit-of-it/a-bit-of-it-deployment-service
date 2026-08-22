using DeploymentService.Application;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentService.Controllers;

[Route("api/[controller]")]
public class CustomerController(ICustomerRepository customerRepository, IImageRepository imageRepository) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Deploy()
    {
        var customers = await customerRepository.GetCustomers();
        var images = await imageRepository.GetDockerImages("website"); // temp
        return Ok(new {customers, images});
    }
}