namespace SCA.Application.Abstractions;

/// <summary>
/// Hides Ollama HTTP details from the Application layer.
/// </summary>
public interface IOllamaClient
{
    /// <summary>
    /// Sends the user message to Ollama and returns the answer text.
    /// </summary>
    Task<string> AskAsync(string message, CancellationToken cancellationToken);

    /// <summary>
    /// Checks whether the configured model exists in Ollama.
    /// </summary>
    Task<bool> IsModelReadyAsync(CancellationToken cancellationToken);
}
