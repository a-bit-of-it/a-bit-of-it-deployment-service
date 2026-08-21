namespace DeploymentService;

public class Config
{
    public const string Organization = "a-bit-of-it";

    public required GitHub GitHub { get; init; }
}

public class GitHub
{
    public required string PackagesToken { get; init; }
}