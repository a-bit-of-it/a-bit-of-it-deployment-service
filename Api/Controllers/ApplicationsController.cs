using Api.Application.Services;
using Api.Controllers.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class ApplicationsController(ApplicationService applicationService) : ControllerBase
{
    [HttpPost("{id:int}/deployments")]
    public async Task<IActionResult> Deploy(int id, [FromBody] DeploymentRequest request)
    {
        await applicationService.Deploy(id, request.Tag);

        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetCustomers()
    {
        var applications = await applicationService.GetAll();
        
        return Ok(applications);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var application = await applicationService.Get(id);
     
        return Ok(application);
    }
    
    [HttpGet("{id:int}/tags")]
    public async Task<IActionResult> GetAvailableTags(int id)
    {
        var tags = await applicationService.GetTags(id);
        
        return Ok(tags);
    }
}