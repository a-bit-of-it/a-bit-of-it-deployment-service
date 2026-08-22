namespace DeploymentService;

public class Config
{
    public const string Organization = "a-bit-of-it";

    public required GitHub GitHub { get; init; }
    public required Ssh Ssh { get; init; }
}

public class GitHub
{
    public required string PackagesToken { get; init; }
}

public class Ssh
{
    public required string Username { get; init; }
    public required string Password { get; init; }
}
