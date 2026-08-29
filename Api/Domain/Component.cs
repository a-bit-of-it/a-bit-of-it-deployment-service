namespace Api.Domain;

public record Component(string Name, string ContainerName, string Image, bool IsRunning, string Status, string Ports);
