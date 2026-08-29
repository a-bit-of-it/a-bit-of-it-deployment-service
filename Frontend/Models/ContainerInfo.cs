namespace Frontend.Models;

public sealed record ContainerInfo(string Name, string ContainerName, string Image, bool IsRunning, string Status, string Ports);
