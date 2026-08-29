using Api.Application;
using Api.Controllers.Requests;
using Api.Domain;
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
    
    [HttpPost("{id:int}/tags")]
    public async Task<IActionResult> GetTags(int id)
    {
        var tag = await applicationService.CreateTag(id);

        return CreatedAtAction(nameof(GetTags), new { id }, tag);
    }
    
    [HttpGet("{id:int}/tags")]
    public async Task<ActionResult<List<Tag>>> GetAvailableTags(int id)
    {
        var tags = await applicationService.GetTags(id);
        
        return Ok(tags);
    }
    
    [HttpGet("{id:int}/workflows/{tag}")]
    public async Task<IActionResult> GetWorkflow(int id, string tag)
    {
        var workflow = await applicationService.GetWorkflow(id, new Tag(tag));
        
        return Ok(workflow);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetApplications()
    {
        var applications = await applicationService.GetAll();
        
        return Ok(applications);
    }
    
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetApplication(int id)
    {
        var application = await applicationService.Get(id);

        return Ok(application);
    }

    [HttpGet("{id:int}/containers")]
    public async Task<IActionResult> GetContainers(int id)
    {
        var containers = await applicationService.GetContainers(id);

        return Ok(containers);
    }

    [HttpGet("{id:int}/releases/latest")]
    public async Task<IActionResult> GetLatestRelease(int id)
    {
        var release = await applicationService.GetLatestRelease(id);

        return Ok(release);
    }

    [HttpGet("{id:int}/releases/{tagName}")]
    public async Task<IActionResult> GetRelease(int id, string tagName)
    {
        var release = await applicationService.GetRelease(id, tagName);

        return Ok(release);
    }
}