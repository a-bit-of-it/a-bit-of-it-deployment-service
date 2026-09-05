using System.Text;
using System.Text.Json;
using Api.Application.Interfaces;
using Api.Domain;
using Api.Infrastructure.Server.DTOs;
using CSharpFunctionalExtensions;
using JetBrains.Annotations;
using Renci.SshNet;

namespace Api.Infrastructure.Server;

[UsedImplicitly]
public class ServerConnection (Config config, ILogger<ServerConnection> logger) : IServerConnection
{
    public async Task<Result> Promote(Domain.Server server, string containerNamePrefix, string remoteDir)
    {
        using var ssh = await Connect(server);

        var result = RunCommand(
            ssh,
            $"cd {remoteDir} && docker compose -p {containerNamePrefix} pull && docker compose -p {containerNamePrefix} up -d --remove-orphans",
            TimeSpan.FromMinutes(5));

        ssh.Disconnect();

        return result.IsFailure ? Result.Failure(result.Error) : Result.Success();
    }

    public async Task<Result<List<Container>>> GetContainers(Domain.Server server)
    {
        using var ssh = await Connect(server);

        var result = RunCommand(ssh, "docker container ls --format '{{json .}}'");

        ssh.Disconnect();

        return result.Map(output => ParseContainers(output).Select(ToContainer).ToList());
    }
    
    public async Task<Result<string>> PushDeploymentConfig (Domain.Server server, string contents, string folder)
    {
        var remoteDir = $"/opt/deployments/{folder}";
        var remoteComposePath = $"{remoteDir}/docker-compose.yml";
        
        using var sftp = new SftpClient(server.Ip, config.Ssh.Username, config.Ssh.Password);
        sftp.Connect();
        
        if (!await sftp.ExistsAsync(remoteDir))
            await EnsureRemoteDirectoryAsync(sftp, remoteDir);

        using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(contents)))
        {
            sftp.UploadFile(stream, remoteComposePath, canOverride: true);
        }

        sftp.Disconnect();

        return remoteDir;
    }
    
    // If more than one folder in a path doesn't exist, you must mkdir each individual folder
    private static async Task EnsureRemoteDirectoryAsync(SftpClient sftp, string remoteDir)
    {
        var segments = remoteDir.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var currentPath = "";

        foreach (var segment in segments)
        {
            currentPath += "/" + segment;

            if (!await sftp.ExistsAsync(currentPath))
            {
                await sftp.CreateDirectoryAsync(currentPath);
            }
        }
    }

    private static Container ToContainer(DockerContainer dto) => new(
        Name: dto.Names,
        ContainerName: dto.Names,
        Image: dto.Image,
        IsRunning: dto.State == "running",
        Status: dto.Status,
        Ports: dto.Ports);

    private static List<DockerContainer> ParseContainers(string dockerLsOutput)
    {
        return dockerLsOutput
            .Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(line => JsonSerializer.Deserialize<DockerContainer>(line))
            .Where(container => container != null)
            .Select(container => container!)
            .ToList();
    }

    private async Task<SshClient> Connect(Domain.Server server)
    {
        logger.LogDebug("Connecting...");

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
