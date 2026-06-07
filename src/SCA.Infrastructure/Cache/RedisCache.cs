using SCA.Application.Abstractions;
using Microsoft.Extensions.Caching.Distributed;

namespace SCA.Infrastructure.Cache;

/// <summary>
/// Small Redis cache wrapper used by the Application layer.
/// </summary>
public sealed class RedisCache(IDistributedCache distributedCache) : IRedisCache
{
    /// <summary>
    /// Reads a string value from Redis.
    /// </summary>
    public Task<string?> GetStringAsync(string key, CancellationToken cancellationToken)
    {
        return distributedCache.GetStringAsync(key, cancellationToken);
    }

    /// <summary>
    /// Writes a string value to Redis with an expiration time.
    /// </summary>
    public Task SetStringAsync(string key, string value, TimeSpan duration, CancellationToken cancellationToken)
    {
        DistributedCacheEntryOptions options = new()
        {
            AbsoluteExpirationRelativeToNow = duration
        };

        return distributedCache.SetStringAsync(key, value, options, cancellationToken);
    }
}
