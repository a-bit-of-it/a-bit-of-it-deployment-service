using Api.Application;
using Api.Controllers.Responses;
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
    
    [HttpGet("fleet-status")]
    public async Task<IActionResult> FleetStatus()
    {
        var servers = await serverService.GetServers();

        var allOnline = servers.All(server => server.IsOnline);

        return Ok(new FleetStatusResponse(allOnline));
    }
    
    [HttpGet("docker-status-test")]
    public async Task<IActionResult> DockerStatusTest()
    {
        var dockerStatusTest = await serverService.GetDockerStuff();

        return Ok(dockerStatusTest);
    }
}