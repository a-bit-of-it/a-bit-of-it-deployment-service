namespace DeploymentService.Exceptions;

public class InstallationException : Exception
{
    public InstallationException(string message) : base(message) {}
}