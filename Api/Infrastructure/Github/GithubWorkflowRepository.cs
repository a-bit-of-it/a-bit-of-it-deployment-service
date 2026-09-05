using Api.Application.Interfaces;
using Api.Domain;
using Api.Infrastructure.Github.DTOs;

namespace Api.Infrastructure.Github;

public class GithubWorkflowRepository(HttpClient client) : IWorkflowRepository
{
    public async Task<Workflow?> GetWorkflow(string repository, Tag tag)
    {
        var workflows = await client
            .GetFromJsonAsync<GithubWorkflows>($"repos/{Config.Organization}/{repository}/actions/runs?event=push&branch={tag.Name}");

        if(workflows == null || workflows.Runs.Count == 0) 
            return null;

        var workflow = workflows.Runs.First();
        return new Workflow(workflow.Id, workflow.Status == "completed", workflow.Conclusion == "success");
    }
}