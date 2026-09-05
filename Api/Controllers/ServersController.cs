using Api.Application;
using Api.Application.Models;
using Api.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/[controller]")]
public class ServersController(ServerService serverService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<Server>>> GetServers()
    {
        var servers = await serverService.GetServers();

        return Ok(servers);
    }
    
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Server>> GetServer(int id)
    {
        var server = await serverService.GetServer(id);

        return Ok(server);
    }
    
    [HttpGet("fleet-status")]
    public async Task<ActionResult<FleetStatus>> FleetStatus()
    {
        var fleetStatus = await serverService.GetFleetStatus();

        return Ok(fleetStatus);
    }

    [HttpGet("{id:int}/containers")]
    public async Task<ActionResult<ServerContainers>> GetContainers(int id)
    {
        var serverContainers = await serverService.GetContainers(id);

        return Ok(serverContainers);
    }
}