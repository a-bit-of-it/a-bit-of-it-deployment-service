using Api.Application;
using Api.Domain;
using CSharpFunctionalExtensions;
using Renci.SshNet;

namespace Api.Infrastructure.Server;

public class SshConnection (Config config) : IServerConnection
{
    public async Task<Result<ServerInterrogationInfo>> InterrogateServer(Domain.Server server)
    {
        using var ssh = new SshClient(server.Ip, config.Ssh.Username, config.Ssh.Password);

        await ssh.ConnectAsync(CancellationToken.None);
        
        var dockerVersion = RunCommand(
            ssh,
            "docker --version");

        // if (command.ExitStatus is not 0)
        //     return Result.Failure<ServerInterrogationInfo>($"Command failed. ExitStatus: {command.ExitStatus}. Error: {command.Error}");
        
        ssh.Disconnect();

        return Result.Success(new ServerInterrogationInfo() {DockerVersion =  dockerVersion, IsOnline = true});
    }
    
    public async Task<Result> PullDockerImage(Domain.Server server, DockerImage image)
    {
        using var ssh = new SshClient(server.Ip, config.Ssh.Username, config.Ssh.Password);

        await ssh.ConnectAsync(CancellationToken.None);

        var cmd = $"docker pull {image.Image}";
        var command = ssh.RunCommand(cmd);

        if (command.ExitStatus is not 0)
            return Result.Failure($"Command failed. ExitStatus: {command.ExitStatus}. Error: {command.Error}");
        
        ssh.Disconnect();

        return Result.Success();
    }

    public async Task DockerPullAndRunAndAllThatStuff(Domain.Server server, string remoteDir)
    {
        using var ssh = new SshClient(server.Ip, config.Ssh.Username, config.Ssh.Password);
        await ssh.ConnectAsync(CancellationToken.None);

        var command = ssh.CreateCommand(
            $"cd {remoteDir} && docker compose pull && docker compose up -d --remove-orphans"
        );
        command.CommandTimeout = TimeSpan.FromMinutes(5);

        var output = command.Execute();

        ssh.Disconnect();

        // logger.LogInformation("Deploy output for {RemoteDir}: {Output}", remoteDir, output);

        if (command.ExitStatus != 0)
        {
            // logger.LogError("Deploy failed for {RemoteDir} (exit {ExitStatus}): {Error}",
            //     remoteDir, command.ExitStatus, command.Error);

            throw new InvalidOperationException(
                $"Deploy failed on {server.Ip} (exit {command.ExitStatus}): {command.Error}");
        }
    }

    public async Task<Result> StartDockerContainer(Domain.Server server, DockerImage image)
    {
        using var ssh = new SshClient(server.Ip, config.Ssh.Username, config.Ssh.Password);

        await ssh.ConnectAsync(CancellationToken.None);

        var cmd = $"stop existing container if it exists... then start the newest image in a container"; // TODO
        var command = ssh.RunCommand(cmd);

        if (command.ExitStatus is not 0)
            return Result.Failure($"Command failed. ExitStatus: {command.ExitStatus}. Error: {command.Error}");
        
        ssh.Disconnect();

        return Result.Success();
    }
    
    private string RunCommand(SshClient ssh, string command)
    {
        var result = ssh.RunCommand(command);

        if (result.ExitStatus != 0)
            throw new Exception(
                $"Command failed: {command}. " +
                $"ExitStatus: {result.ExitStatus}. " +
                $"Error: {result.Error}");

        return result.Result.Trim();
    }
}
