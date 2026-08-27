namespace Api.Domain;

public record Release(long Id, string Name, DateTime CreatedAt, string Url);