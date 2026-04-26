using Common.Exceptions;
using Common.Response;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Middleware;

public class HttpExceptionMiddleware(RequestDelegate next, ILogger<HttpExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<HttpExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (HttpException ex)
        {
            await WriteError(context, ex.Code, ex.Message, ex.Status);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteError(context, "INTERNAL_ERROR", "Internal server error", 500);
        }
    }

    private static async Task WriteError(HttpContext context, string code, string message, int status)
    {
        var response = HttpApiResponse<object>.Fail(null, code, message);

        context.Response.StatusCode = status;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(response);
    }
}
