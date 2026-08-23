namespace Api.Exceptions;

public class InstallationException : Exception
{
    public InstallationException(string message) : base(message) {}
}