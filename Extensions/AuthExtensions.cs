using Common.Response;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text.Json;

namespace Common.Extensions;

public static class AuthExtensions
{
    public static IServiceCollection AddKeycloakJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = GetRequiredConfig(configuration, "Keycloak:Authority");
        var audience = GetRequiredConfig(configuration, "Keycloak:Audience");

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Authority = authority;
                options.RequireHttpsMetadata = authority.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                ValidateIssuer = true,
                ValidIssuer = authority,
                ValidateAudience = true,
                ValidAudience = audience,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                RoleClaimType = ClaimTypes.Role
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal?.Identity is not ClaimsIdentity identity)
                            return Task.CompletedTask;

                        AddRealmRoles(identity);
                        AddClientRoles(identity, audience);

                        return Task.CompletedTask;
                    }
                };
            });

        services.PostConfigureAll<JwtBearerOptions>(options =>
        {
            options.Events ??= new JwtBearerEvents();

            var previousOnChallenge = options.Events.OnChallenge;
            var previousOnForbidden = options.Events.OnForbidden;

            options.Events.OnChallenge = async context =>
            {
                if (previousOnChallenge is not null)
                {
                    await previousOnChallenge(context);
                }

                if (context.Response.HasStarted)
                {
                    return;
                }

                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(
                    HttpApiResponse<object?>.Fail(
                        data: null,
                        code: "UNAUTHORIZED",
                        message: "Unauthorized"
                    )
                );
            };

            options.Events.OnForbidden = async context =>
            {
                if (previousOnForbidden is not null)
                {
                    await previousOnForbidden(context);
                }

                if (context.Response.HasStarted)
                {
                    return;
                }

                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(
                    HttpApiResponse<object?>.Fail(
                        data: null,
                        code: "FORBIDDEN",
                        message: "Forbidden"
                    )
                );
            };
        });

        services.AddAuthorization();
        return services;
    }

    private static void AddRealmRoles(ClaimsIdentity identity)
    {
        var raw = identity.FindFirst("realm_access")?.Value;

        if (string.IsNullOrWhiteSpace(raw)) return;

        using var doc = JsonDocument.Parse(raw);

        if (!doc.RootElement.TryGetProperty("roles", out var roles)) return;

        foreach (var role in roles.EnumerateArray())
        {
            var value = role.GetString();
            if (!string.IsNullOrWhiteSpace(value))
                identity.AddClaim(new Claim(ClaimTypes.Role, value));
        }
    }

    private static void AddClientRoles(ClaimsIdentity identity, string clientId)
    {
        var raw = identity.FindFirst("resource_access")?.Value;

        if (string.IsNullOrWhiteSpace(raw)) return;

        using var doc = JsonDocument.Parse(raw);

        if (!doc.RootElement.TryGetProperty(clientId, out var client)) return;

        if (!client.TryGetProperty("roles", out var roles)) return;
        foreach (var role in roles.EnumerateArray())
        {
            var value = role.GetString();
            if (!string.IsNullOrWhiteSpace(value))
                identity.AddClaim(new Claim(ClaimTypes.Role, value));
        }
        
    }

    private static string GetRequiredConfig(IConfiguration configuration, string key)
    {
        var value = configuration[key];
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidOperationException($"Missing required configuration: {key}");

        return value;
    }
}
