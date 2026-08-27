using System.Text.Json.Serialization;

namespace Api.Infrastructure.Github.DTOs;

public record GithubRelease(long Id, string Name, [property: JsonPropertyName("created_at")] DateTime CreatedAt, [property: JsonPropertyName("html_url")] string Url);