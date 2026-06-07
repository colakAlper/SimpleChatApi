using CB.Application.Abstractions;
using CB.IntegrationTests.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace CB.IntegrationTests;

/// <summary>
/// Test host with fake Redis and fake Ollama services.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    /// <summary>
    /// Fake Ollama client used by tests.
    /// </summary>
    public CountingOllamaClient OllamaClient { get; } = new();

    /// <summary>
    /// Fake Redis cache used by tests.
    /// </summary>
    public InMemoryRedisCache RedisCache { get; } = new();

    /// <summary>
    /// Replaces external services with test doubles.
    /// </summary>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<IOllamaClient>();
            services.RemoveAll<IRedisCache>();

            services.AddSingleton<IOllamaClient>(OllamaClient);
            services.AddSingleton<IRedisCache>(RedisCache);
        });
    }
}
