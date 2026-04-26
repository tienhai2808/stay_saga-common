// src/Middleware/RequestIdMiddleware.cs
using Microsoft.AspNetCore.Http;

namespace Common.Middleware;

public class HttpRequestIdMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var requestId = context.Request.Headers["X-Request-Id"].FirstOrDefault() ?? Guid.NewGuid().ToString();

        context.Items["RequestId"] = requestId;
        context.Response.Headers["X-Request-Id"] = requestId;

        await _next(context);
    }
}