using DeploymentService.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeploymentService.Controllers;

[Route("api/[controller]")]
public class ServersController(ServerService serverService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetServers()
    {
        var servers = await serverService.GetServers();

        return Ok(servers);
    }
}