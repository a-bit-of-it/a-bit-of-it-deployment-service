using Api.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class ApplicationsController(ApplicationService applicationService) : ControllerBase
{
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
}