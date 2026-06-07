using SCA.Application.Abstractions;
using SCA.Infrastructure.Cache;
using SCA.Infrastructure.Ollama;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace SCA.Infrastructure;

/// <summary>
/// Registers Infrastructure layer services.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Adds Redis cache and Ollama HTTP client.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        RedisCacheOptions redisOptions = configuration.GetRequiredOptions<RedisCacheOptions>(RedisCacheOptions.SectionName);
        OllamaOptions ollamaOptions = configuration.GetRequiredOptions<OllamaOptions>(OllamaOptions.SectionName);

        redisOptions.Validate();
        ollamaOptions.Validate();

        services.AddSingleton(redisOptions);
        services.AddSingleton(ollamaOptions);

        services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            ILogger logger = serviceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("RedisConnection");

            try
            {
                return ConnectionMultiplexer.Connect(redisOptions.ConnectionString);
            }
            catch (Exception exception)
            {
                logger.LogWarning(exception, "Redis connection could not be created.");
                throw new InvalidOperationException("Redis connection could not be created.", exception);
            }
        });

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisOptions.ConnectionString;
            options.InstanceName = redisOptions.InstanceName;
        });

        services.AddScoped<IRedisCache, RedisCache>();

        services.AddHttpClient<IOllamaClient, OllamaClient>((_, client) =>
        {
            client.BaseAddress = new Uri(ollamaOptions.BaseUrl);
            client.Timeout = TimeSpan.FromSeconds(ollamaOptions.TimeoutSeconds);
        });

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
