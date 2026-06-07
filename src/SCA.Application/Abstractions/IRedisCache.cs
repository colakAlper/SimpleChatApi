namespace SCA.Application.Abstractions;

/// <summary>
/// Hides Redis details from the Application layer.
/// </summary>
public interface IRedisCache
{
    /// <summary>
    /// Reads a string value from cache.
    /// </summary>
    Task<string?> GetStringAsync(string key, CancellationToken cancellationToken);

    /// <summary>
    /// Writes a string value to cache for a limited time.
    /// </summary>
    Task SetStringAsync(string key, string value, TimeSpan duration, CancellationToken cancellationToken);
}
