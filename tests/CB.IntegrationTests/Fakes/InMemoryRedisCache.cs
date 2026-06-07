using CB.Application.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace CB.IntegrationTests.Fakes;

/// <summary>
/// In-memory cache used instead of Redis during integration tests.
/// </summary>
public sealed class InMemoryRedisCache : IRedisCache
{
    private readonly MemoryCache _cache = new(new MemoryCacheOptions());

    /// <summary>
    /// Reads a cached value from memory.
    /// </summary>
    public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_cache.TryGetValue(key, out string? value) ? value : null);
    }

    /// <summary>
    /// Writes a cached value to memory.
    /// </summary>
    public Task SetStringAsync(string key, string value, TimeSpan duration, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _cache.Set(key, value, duration);
        return Task.CompletedTask;
    }
}
