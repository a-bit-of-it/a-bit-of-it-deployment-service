using JetBrains.Annotations;

namespace DeploymentService.Domain;

[UsedImplicitly]
public record DockerImage (long Id, string Image, DateTimeOffset UpdatedAt);