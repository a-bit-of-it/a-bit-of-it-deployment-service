namespace Api.Domain;

public class DockerStatusInfo
{
    public string DockerVersion { get; set; } = "";
    public List<ContainerInfo> Containers { get; set; } = new();
}
