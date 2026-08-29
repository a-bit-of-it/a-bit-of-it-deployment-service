using System.Text.Json.Serialization;

namespace Api.Domain;

public class ContainerInfo
{
    [JsonPropertyName("ID")]
    public string Id { get; set; } = "";

    [JsonPropertyName("Image")]
    public string Image { get; set; } = "";

    [JsonPropertyName("Names")]
    public string Names { get; set; } = "";

    [JsonPropertyName("State")]
    public string State { get; set; } = "";

    [JsonPropertyName("Status")]
    public string Status { get; set; } = "";

    [JsonPropertyName("Ports")]
    public string Ports { get; set; } = "";
}
