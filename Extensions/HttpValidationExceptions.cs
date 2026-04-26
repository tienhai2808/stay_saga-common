using Common.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

public static class ApiValidationExtensions
{
    public static IServiceCollection AddApiControllers(this IServiceCollection services)
    {
        services
            .AddControllers()
            .ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(x => x.Value is { Errors.Count: > 0 })
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value!.Errors
                                .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage)
                                .ToArray()
                        );

                    var response = HttpApiResponse<Dictionary<string, string[]>>.Fail(
                        errors,
                        "VALIDATION_ERROR",
                        "Invalid input data"
                    );

                    return new BadRequestObjectResult(response);
                };
            });

        return services;
    }
}
