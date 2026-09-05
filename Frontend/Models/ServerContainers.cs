namespace Frontend.Models;

public sealed record ServerContainers(List<CustomerContainers> Customers);

public sealed record CustomerContainers(int CustomerId, string CustomerName, List<ApplicationContainers> Applications);

public sealed record ApplicationContainers(int ApplicationId, string ApplicationName, List<ApplicationContainer> Containers);
public sealed record ApplicationContainer(string Name, string ContainerName, string Image, bool IsRunning, string Status, string Ports);
