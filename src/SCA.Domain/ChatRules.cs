namespace SCA.Domain;

/// <summary>
/// Small business rules used by the chat feature.
/// </summary>
public static class ChatRules
{
    /// <summary>
    /// Maximum accepted user message length.
    /// </summary>
    public const int MessageMaxLength = 500;

    /// <summary>
    /// How long the same question stays in Redis cache.
    /// </summary>
    public static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
}
