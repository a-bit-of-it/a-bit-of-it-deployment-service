using DeploymentService.Application;
using DeploymentService.Domain;
using Renci.SshNet;

namespace DeploymentService.Infrastructure.Server;

public class SshConnection (Config config) : IServerConnection
{
    public async Task PullDockerImage(Domain.Server server, DockerImage image)
    {
        using var ssh = new SshClient(server.Ip, config.Ssh.Username, config.Ssh.Password);

        await ssh.ConnectAsync(CancellationToken.None);

        var cmd = $"docker pull {image.Image}";
        var command = ssh.RunCommand(cmd);

        Console.WriteLine(command.Result);
        Console.WriteLine($"Exit status: {command.ExitStatus}");

        ssh.Disconnect();
        
        ////docker pull ghcr.io/a-bit-of-it/website:sha-7a6c8ab
    }
}
