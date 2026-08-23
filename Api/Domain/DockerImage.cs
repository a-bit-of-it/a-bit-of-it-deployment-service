using JetBrains.Annotations;

namespace Api.Domain;

[UsedImplicitly]
public record DockerImage (long Id, string Image, DateTimeOffset UpdatedAt);