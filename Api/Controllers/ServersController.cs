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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetServer(int id)
    {
        var server = await serverService.GetServer(id);

        return Ok(server);
    }

    [HttpGet("{id:int}/components")]
    public async Task<IActionResult> GetComponents(int id)
    {
        var components = await serverService.GetComponents(id);

        return Ok(components);
    }
}