namespace CB.Infrastructure.Ollama.Models;

/// <summary>
/// Response body returned by Ollama chat endpoint.
/// </summary>
/// <param name="Message">Generated Ollama message.</param>
public sealed record OllamaChatResponse(OllamaMessage? Message);
