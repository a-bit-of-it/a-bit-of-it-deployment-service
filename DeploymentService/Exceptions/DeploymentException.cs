namespace DeploymentService.Exceptions;

public class DeploymentException : Exception
{
    public DeploymentException(string message) : base(message) {}
}