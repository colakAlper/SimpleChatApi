namespace CB.Infrastructure.Ollama.Models;

/// <summary>
/// Single chat message used by Ollama.
/// </summary>
/// <param name="Role">Message role, such as system or user.</param>
/// <param name="Content">Message content.</param>
public sealed record OllamaMessage(string Role, string Content);
