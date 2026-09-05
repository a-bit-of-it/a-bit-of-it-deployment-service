namespace Api.Application.Models;

public record FleetStatus (bool IsOnline, List<ServerStatus> Statuses);