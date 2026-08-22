namespace DeploymentService.Domain;

public record Application(int Id, string Name, string Registry, string Url, Server Server);