namespace SCA.Infrastructure.Ollama.Models;

/// <summary>
/// Model list returned by Ollama.
/// </summary>
/// <param name="Models">Installed model list.</param>
public sealed record OllamaTagsResponse(IReadOnlyCollection<OllamaModel> Models);
