using JetBrains.Annotations;

namespace Api.Infrastructure.Github.DTOs;

[UsedImplicitly]
public record GithubTag(string Name, GithubTagCommit Commit);

[UsedImplicitly]
public record GithubTagCommit(string Sha);