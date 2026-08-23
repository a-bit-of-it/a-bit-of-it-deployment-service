namespace Api.Controllers.Responses;

public record CustomerResponse (int Id, List<ApplicationResponse> Applications);