using Api.Application;
using Api.Controllers.Requests;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/applications/{applicationId:int}/deployments")]
public class DeploymentsController(DeploymentService deploymentService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Deploy(int applicationId, [FromBody] DeploymentRequest request)
    {
        await deploymentService.Deploy(applicationId, request.Tag);

        return Ok();
    }
}
