using Api.Domain;

namespace Api.Application.Interfaces;

public interface IReleaseRepository
{
    Task<Release> CreateRelease(string repository, string tagName);
}