namespace Api.Infrastructure.Github.DTOs;

public record GithubTag(
    string Name,
    GithubTagCommit Commit
);

public record GithubTagCommit(
    string Sha
);