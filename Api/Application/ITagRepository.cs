using Api.Domain;

namespace Api.Application;

public interface ITagRepository
{
    Task<List<Tag>> GetTags(string repository);
}