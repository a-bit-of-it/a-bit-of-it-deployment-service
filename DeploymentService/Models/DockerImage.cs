using JetBrains.Annotations;

namespace DeploymentService.Models;

[UsedImplicitly]
public record DockerImage (string Image, DateTimeOffset UpdatedAt);