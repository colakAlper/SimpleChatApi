using System.Text.Json;
using SCA.Api.Authentication;
using SCA.Api.Common.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace SCA.Api.Extensions;

/// <summary>
/// Registers API layer services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds controllers, authentication and FluentValidation.
    /// </summary>
    public static IServiceCollection AddApiLayer(this IServiceCollection services, IConfiguration configuration)
    {
        BasicAuthOptions basicAuthOptions = configuration.GetRequiredOptions<BasicAuthOptions>(BasicAuthOptions.SectionName);
        basicAuthOptions.Validate();

        services.AddSingleton(basicAuthOptions);

        services
            .AddAuthentication(BasicAuthenticationHandler.SchemeName)
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>(BasicAuthenticationHandler.SchemeName, _ => { });

        services.AddAuthorization();

        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

        // Validations for the dto's
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                Dictionary<string, string[]> errors = context.ModelState
                    .Where(x => x.Value?.Errors.Count > 0)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Value!.Errors.Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Invalid value."
                            : error.ErrorMessage).ToArray());

                return new BadRequestObjectResult(ApiResponse.Fail("Validation failed.", errors));
            };
        });

        services.AddValidatorsFromAssemblyContaining<Program>();
        services.AddHealthChecks()
            .AddCheck<SCA.Infrastructure.HealthChecks.RedisHealthCheck>("redis")
            .AddCheck<SCA.Infrastructure.HealthChecks.OllamaHealthCheck>("ollama");

        return services;
    }

    private static TOptions GetRequiredOptions<TOptions>(this IConfiguration configuration, string sectionName)
        where TOptions : class, new()
    {
        IConfigurationSection section = configuration.GetSection(sectionName);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"'{sectionName}' configuration section is missing.");
        }

        return section.Get<TOptions>() ?? throw new InvalidOperationException($"'{sectionName}' configuration section cannot be read.");
    }
}
