namespace Frontend.Models;

public record FleetStatus(bool IsOnline, List<ServerStatus> Statuses);

public record ServerStatus(int Id, bool IsOnline, string? Error);
