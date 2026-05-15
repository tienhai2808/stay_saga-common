using Common.DTOs;
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
                    var firstErrorMessage = context.ModelState
                        .Where(x => x.Value is { Errors.Count: > 0 })
                        .SelectMany(
                            kvp => kvp.Value!.Errors.Select(
                                e => string.IsNullOrWhiteSpace(e.ErrorMessage) ? "Invalid value." : e.ErrorMessage
                            )
                        )
                        .FirstOrDefault();

                    var response = HttpApiResponseDto<object?>.Fail(
                        null,
                        "BAD_REQUEST",
                        string.IsNullOrWhiteSpace(firstErrorMessage) ? "Invalid input data" : firstErrorMessage
                    );

                    return new BadRequestObjectResult(response);
                };
            });

        return services;
    }
}
