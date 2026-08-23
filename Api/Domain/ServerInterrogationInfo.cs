namespace Api.Domain;

public class ServerInterrogationInfo
{
    public int ServerId  { get; set; } 
    public bool IsOnline { get; set; }
    public string DockerVersion { get; set; }
    private string PendingUpdates;
    //etc.
}