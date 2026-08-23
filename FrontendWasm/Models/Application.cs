namespace FrontendWasm.Models;

public sealed class Application
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Registry { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public Server Server { get; set; } = new();
}