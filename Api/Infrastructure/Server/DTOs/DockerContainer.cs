using System.Text.Json.Serialization;

namespace Api.Infrastructure.Server.DTOs;

public record DockerContainer(
    [property: JsonPropertyName("ID")] string Id,
    [property: JsonPropertyName("Image")] string Image,
    [property: JsonPropertyName("Names")] string Names,
    [property: JsonPropertyName("State")] string State,
    [property: JsonPropertyName("Status")] string Status,
    [property: JsonPropertyName("Ports")] string Ports
);
