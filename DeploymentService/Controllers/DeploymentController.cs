using Microsoft.AspNetCore.Mvc;

namespace DeploymentService.Controllers;

[Route("api/[controller]")]
public class DeploymentController(Application.Services.DeploymentService deploymentService) : ControllerBase
{
    public class DeploymentRequest
    {
        public required int CustomerId { get; init; }
        public required int ApplicationId { get; init; }
        public required long ImageId { get; set; }
    }
    
    [HttpPost]
    public async Task<IActionResult> Deploy([FromBody] DeploymentRequest request)
    {
        await deploymentService.Deploy(
            request.CustomerId,
            request.ApplicationId,
            request.ImageId);

        return Ok();
    }
}