namespace SCA.Infrastructure.Ollama;

/// <summary>
/// Ollama connection settings from appsettings.json.
/// </summary>
public sealed class OllamaOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Ollama";

    /// <summary>
    /// Ollama service URL.
    /// </summary>
    public string BaseUrl { get; init; } = string.Empty;

    /// <summary>
    /// Local model name used by the API.
    /// </summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>
    /// HTTP timeout in seconds.
    /// </summary>
    public int TimeoutSeconds { get; init; } = 120;

    /// <summary>
    /// Stops the application early when Ollama settings are invalid.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl) || !Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
        {
            throw new InvalidOperationException("Ollama:BaseUrl must be a valid URL.");
        }

        if (string.IsNullOrWhiteSpace(Model))
        {
            throw new InvalidOperationException("Ollama:Model cannot be empty.");
        }

        if (TimeoutSeconds <= 0)
        {
            throw new InvalidOperationException("Ollama:TimeoutSeconds must be greater than zero.");
        }
    }
}
