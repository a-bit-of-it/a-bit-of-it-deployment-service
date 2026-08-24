namespace Api.Application.Interfaces;

public interface IFilePusher
{
    Task<string> Push(Domain.Server server, string contents, string folder);
}