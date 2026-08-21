using DeploymentService.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentService.Controllers;

[Route("api/[controller]")]
public class DeploymentController(IGithubService githubService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var images = await githubService.GetDockerImages("website");
        return Ok(images);
    }
}