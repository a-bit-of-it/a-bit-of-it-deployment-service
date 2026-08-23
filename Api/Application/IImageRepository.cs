using Api.Domain;

namespace Api.Application;

public interface IImageRepository
{
    Task<List<DockerImage>> GetDockerImages(string repository);
    Task<DockerImage?> GetDockerImage(string repository, long imageId);
}