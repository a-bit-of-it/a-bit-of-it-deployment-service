using Api.Domain;

namespace Api.Controllers.Responses;

public record ApplicationResponse (int Id, List<DockerImage> DockerImages);