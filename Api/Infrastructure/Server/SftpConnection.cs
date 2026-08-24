using System.Text;
using Api.Application.Interfaces;
using Renci.SshNet;

namespace Api.Infrastructure.Server;

public class SftpConnection (Config config) : IFilePusher
{
    public async Task<string> Push (Domain.Server server, string contents, string folder)
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
}