using Api.Exceptions;

namespace Api.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (NotFoundException ex)
        {
            logger.LogWarning(ex, "Resource not found");

            context.Response.StatusCode = StatusCodes.Status404NotFound;

            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message
            });
        }        
        catch (DeploymentException ex)
        {
            logger.LogWarning(ex, "Deployment failed");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message
            });
        }
        catch (InstallationException ex)
        {
            logger.LogWarning(ex, "Installation failed");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");

            context.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred."
            });
        }
    }
}