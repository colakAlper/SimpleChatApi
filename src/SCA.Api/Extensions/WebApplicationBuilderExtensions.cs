using SCA.Application;
using SCA.Infrastructure;
using Serilog;

namespace SCA.Api.Extensions;

/// <summary>
/// Small builder extensions used by Program.cs.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    /// <summary>
    /// Configures Serilog from appsettings.json.
    /// </summary>
    public static WebApplicationBuilder UseProjectSerilog(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, _, loggerConfiguration) =>
        {
            loggerConfiguration.ReadFrom.Configuration(context.Configuration);
        });

        return builder;
    }

    /// <summary>
    /// Registers API, Application and Infrastructure layers.
    /// </summary>
    public static IServiceCollection AddProjectLayers(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddApiLayer(configuration)
            .AddApplication()
            .AddInfrastructure(configuration);

        return services;
    }
}
