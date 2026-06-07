namespace SCA.Infrastructure.Cache;

/// <summary>
/// Redis cache settings from appsettings.json.
/// </summary>
public sealed class RedisCacheOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Redis";

    /// <summary>
    /// Redis connection string.
    /// </summary>
    public string ConnectionString { get; init; } = string.Empty;

    /// <summary>
    /// Prefix used for Redis keys.
    /// </summary>
    public string InstanceName { get; init; } = "cb:";

    /// <summary>
    /// Stops the application early when Redis settings are missing.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ConnectionString))
        {
            throw new InvalidOperationException("Redis:ConnectionString cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(InstanceName))
        {
            throw new InvalidOperationException("Redis:InstanceName cannot be empty.");
        }
    }
}
