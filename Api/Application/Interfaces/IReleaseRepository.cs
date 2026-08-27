using Api.Domain;

namespace Api.Application.Interfaces;

public interface IReleaseRepository
{
    Task<Release> CreateRelease(string repository, string tagName);
    Task<Release?> GetRelease(string repository, string tagName);
    Task<Release?> GetLatestRelease(string repository);
}