using Api.Domain;

namespace Api.Application.Interfaces;

public interface IWorkflowRepository
{
    Task<Workflow?> GetWorkflow(string repository, Tag tag);
}