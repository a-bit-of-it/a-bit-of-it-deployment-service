namespace Api.Domain;

public record Application(int Id, string Name, string Repository, string Url, Server Server);