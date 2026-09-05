namespace Api.Domain;

public record Container(string Name, string ContainerName, string Image, bool IsRunning, string Status, string Ports);
