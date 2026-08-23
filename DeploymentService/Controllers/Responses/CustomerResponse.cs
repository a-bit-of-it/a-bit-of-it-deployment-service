namespace DeploymentService.Controllers.Responses;

public record CustomerResponse (int Id, List<ApplicationResponse> Applications);