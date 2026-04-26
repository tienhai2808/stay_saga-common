using Common.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Common.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseStaySagaDefaults(this IApplicationBuilder app)
    {
        app.UseMiddleware<HttpRequestIdMiddleware>();
        app.UseMiddleware<HttpExceptionMiddleware>();
        return app;
    }
}