namespace Api.Application;

public interface IFilePusher
{
    Task<string> Push(Domain.Server server, string contents, string slug);
}