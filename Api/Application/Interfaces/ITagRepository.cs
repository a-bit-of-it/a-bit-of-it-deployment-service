using Api.Domain;

namespace Api.Application.Interfaces;

public interface ITagRepository
{
    Task<List<Tag>> GetTags(string repository);
    Task<Tag> CreateTag(string repository);
}