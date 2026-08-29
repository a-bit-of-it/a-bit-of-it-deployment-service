namespace Frontend.Models;

public sealed record ServerComponents(List<CustomerComponents> Customers, List<ContainerInfo> Unassigned);

public sealed record CustomerComponents(int CustomerId, string CustomerName, List<ApplicationComponents> Applications);

public sealed record ApplicationComponents(int ApplicationId, string ApplicationName, List<ContainerInfo> Components);
