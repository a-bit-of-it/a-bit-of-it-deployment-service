using DeploymentService.Models;

namespace DeploymentService.Services;

public interface IGithubService
{
    Task<List<DockerImage>> GetDockerImages(string repository);
}