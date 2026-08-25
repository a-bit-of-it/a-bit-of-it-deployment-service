using Api.Application.Interfaces;
using Api.Domain;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubWorkflowRepository(HttpClient client, ILogger<GithubWorkflowRepository> logger) : IWorkflowRepository
{
    public async Task<Workflow?> GetWorkflow(string repository, string commitSha)
    {
        var workflows = await client
            .GetFromJsonAsync<GithubWorkflows>($"repos/{Config.Organization}/{repository}/actions/runs?event=push&head_sha={commitSha}");

        if(workflows == null) 
            return null;

        var workflow = workflows.Runs.First();
        return new Workflow(workflow.Id, workflow.Status == "completed", workflow.Conclusion == "success");
    }
}