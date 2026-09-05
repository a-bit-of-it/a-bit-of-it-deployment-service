namespace Api.Application.Models;

public record ServerStatus (int Id, bool IsOnline, string? Error);