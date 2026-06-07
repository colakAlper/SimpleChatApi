using System.Text.Json;
using CB.Api.Authentication;
using CB.Api.Common.Responses;
using CB.Api.Features.Chat.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Extensions;

namespace CB.Api.Extensions;

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

        services.AddValidatorsFromAssemblyContaining<ChatRequestValidator>();
        services.AddFluentValidationAutoValidation();
        services.AddHealthChecks();

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
