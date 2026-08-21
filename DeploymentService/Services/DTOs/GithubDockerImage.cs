using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace DeploymentService.Services.DTOs;

[UsedImplicitly]
public record GithubDockerImage(string Name, [property: JsonPropertyName("updated_at")] DateTimeOffset UpdatedAt);