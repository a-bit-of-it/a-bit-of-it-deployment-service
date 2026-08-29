namespace Api.Domain;

public record ServerComponents(List<CustomerComponents> Customers, List<Component> Unassigned);

public record CustomerComponents(int CustomerId, string CustomerName, List<ApplicationComponents> Applications);

public record ApplicationComponents(int ApplicationId, string ApplicationName, List<Component> Components);
