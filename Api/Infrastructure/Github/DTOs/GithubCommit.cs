namespace Api.Infrastructure.Github.DTOs;

public record GithubCommit
{
    public string Sha { get; set; }
}