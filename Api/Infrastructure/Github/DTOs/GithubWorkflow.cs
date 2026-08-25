using System.Text.Json.Serialization;

namespace Api.Infrastructure.Github.DTOs;

public record GithubWorkflows([property: JsonPropertyName("workflow_runs")] List<GithubWorkflow> Runs);

public record GithubWorkflow(long Id, string Status, string Conclusion);
