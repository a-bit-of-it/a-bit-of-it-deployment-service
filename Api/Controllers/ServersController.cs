using Api.Application;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

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