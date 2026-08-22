using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace DeploymentService.Infrastructure.Github.DTOs;

[UsedImplicitly]
public record GithubDockerImage(long Id, string Name, [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt);