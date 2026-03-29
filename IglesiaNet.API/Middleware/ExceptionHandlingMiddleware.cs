using IglesiaNet.Domain.Shared;
using System.Text.Json;

namespace IglesiaNet.API.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (DomainException ex)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            ctx.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new { message = ex.Message });
            await ctx.Response.WriteAsync(body);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error inesperado: {Message}", ex.Message);
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";
            var body = JsonSerializer.Serialize(new { message = "Error interno del servidor. Intenta de nuevo." });
            await ctx.Response.WriteAsync(body);
        }
    }
}
