namespace Api.Application.Models;

public record ServerContainers(List<CustomerContainers> Customers);

public record CustomerContainers(int CustomerId, string CustomerName, List<ApplicationContainers> Applications);

public record ApplicationContainers(int ApplicationId, string ApplicationName, List<ApplicationContainer> Containers);
public record ApplicationContainer(string Name, string ContainerName, string Image, bool IsRunning, string Status, string Ports);
