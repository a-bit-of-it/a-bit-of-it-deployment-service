namespace DeploymentService.Domain;

public record Customer(int Id, string Name, string Address, string Country, string City, string PhoneNumber, List<Application> Applications);