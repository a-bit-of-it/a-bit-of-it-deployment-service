namespace Frontend.Models;

public sealed record Workflow(long Id, bool IsComplete, bool IsSuccessful)
{
    public bool IsCompletedSuccessfully => IsSuccessful;
}