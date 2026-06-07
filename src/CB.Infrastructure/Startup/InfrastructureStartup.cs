using CB.Application.Abstractions;
using CB.Infrastructure.Ollama;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace CB.Infrastructure.Startup;

/// <summary>
/// Validates external dependencies before the API starts.
/// </summary>
public static class InfrastructureStartup
{
    /// <summary>
    /// Checks Redis connection and the configured Ollama model.
    /// </summary>
    public static async Task ValidateInfrastructureAsync(this IServiceProvider services, CancellationToken cancellationToken = default)
    {
        using IServiceScope scope = services.CreateScope();
        ILogger logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("InfrastructureStartup");

        await ValidateRedisAsync(scope.ServiceProvider, logger);
        await ValidateOllamaAsync(scope.ServiceProvider, logger, cancellationToken);
    }

    private static async Task ValidateRedisAsync(IServiceProvider serviceProvider, ILogger logger)
    {
        try
        {
            IConnectionMultiplexer redis = serviceProvider.GetRequiredService<IConnectionMultiplexer>();
            await redis.GetDatabase().PingAsync();
            logger.LogInformation("Redis connection is ready.");
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Redis connection failed. API will not start.");
            throw new InvalidOperationException("Redis connection failed.", exception);
        }
    }

    private static async Task ValidateOllamaAsync(IServiceProvider serviceProvider, ILogger logger, CancellationToken cancellationToken)
    {
        OllamaOptions options = serviceProvider.GetRequiredService<OllamaOptions>();
        IOllamaClient ollama = serviceProvider.GetRequiredService<IOllamaClient>();
        
        bool modelReady = await ollama.IsModelReadyAsync(cancellationToken);
        if (!modelReady)
        {
            string command = $"docker compose exec ollama ollama pull {options.Model}";

            logger.LogWarning("Ollama model is missing: {Model}", options.Model);
            logger.LogWarning("Run this command before starting the API: {Command}", command);

            // The API should fail early instead of surprising us on the first request.
            throw new InvalidOperationException($"Ollama model is not ready. Run this command first: {command}");
        }
        
        logger.LogInformation("Ollama model is ready: {Model}", options.Model);
    }
}
