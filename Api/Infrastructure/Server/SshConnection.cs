using Api.Application.Interfaces;
using Api.Domain;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Renci.SshNet;

namespace Api.Infrastructure.Server;

[UsedImplicitly]
public class SshConnection (Config config, ILogger<SshConnection> logger) : IServerConnection
{
    public async Task<Result> Deploy(Domain.Server server, string containerNamePrefix, string remoteDir)
    {
        using var ssh = await Connect(server);

        var result = RunCommand(
            ssh,
            $"cd {remoteDir} && docker compose -p {containerNamePrefix} pull && docker compose -p {containerNamePrefix} up -d --remove-orphans",
            TimeSpan.FromMinutes(5));

        ssh.Disconnect();

        return result.IsFailure ? Result.Failure(result.Error) : Result.Success();
    }
    
    public async Task<Result<ServerInterrogationInfo>> InterrogateServer(Domain.Server server)
    {
        using var ssh = await Connect(server);

        var result = RunCommand(ssh, "docker --version");

        ssh.Disconnect();

        return result.Map(dockerVersion => new ServerInterrogationInfo { DockerVersion = dockerVersion, IsOnline = true });
    }
    
    private async Task<SshClient> Connect(Domain.Server server)
    {
        logger.LogDebug("Connecting in...");

        var ssh = new SshClient(server.Ip, config.Ssh.Username, config.Ssh.Password);
        await ssh.ConnectAsync(CancellationToken.None);

        logger.LogDebug("Connected");

        return ssh;
    }

    private static Result<string> RunCommand(SshClient ssh, string commandText, TimeSpan? timeout = null)
    {
        var command = ssh.CreateCommand(commandText);
        if (timeout.HasValue)
            command.CommandTimeout = timeout.Value;

        var output = command.Execute();

        if (command.ExitStatus != 0)
            return Result.Failure<string>(
                $"Command failed on {ssh.ConnectionInfo.Host}: {commandText} (exit {command.ExitStatus}): {command.Error}");

        return Result.Success(output.Trim());
    }
}
