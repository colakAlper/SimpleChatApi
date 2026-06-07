namespace SCA.Infrastructure.Ollama.Models;

/// <summary>
/// Request body sent to Ollama chat endpoint.
/// </summary>
/// <param name="Model">Local model name.</param>
/// <param name="Stream">False means Ollama returns a single JSON response.</param>
/// <param name="Messages">System and user messages.</param>
public sealed record OllamaChatRequest(string Model, bool Stream, IReadOnlyCollection<OllamaMessage> Messages);
