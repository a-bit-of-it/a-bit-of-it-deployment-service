namespace Api.Application.Interfaces;

public interface IDeploymentConfigFetcher
{
    Task<string> GetComposeFileAsync(string repository, string tag);
}