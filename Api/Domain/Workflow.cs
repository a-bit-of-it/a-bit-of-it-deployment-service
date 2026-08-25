namespace Api.Domain;


public record Workflow(long Id, string Status, string Conclusion)
{
    public bool IsCompletedSuccessfully => Status == "completed" &&  Conclusion == "success";
}