namespace SCA.Infrastructure.Ollama.Models;

/// <summary>
/// Model item returned by Ollama tags endpoint.
/// </summary>
/// <param name="Name">Model name.</param>
/// <param name="Model">Model alias.</param>
public sealed record OllamaModel(string Name, string Model);
