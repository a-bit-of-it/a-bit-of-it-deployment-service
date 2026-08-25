using Api.Domain;

namespace Api.Controllers.Requests;

public class DeploymentRequest
{
    public required Tag Tag { get; init; }
}