using DeploymentService.Domain;

namespace DeploymentService.Controllers.Responses;

public record ApplicationResponse (int Id, List<DockerImage> DockerImages);