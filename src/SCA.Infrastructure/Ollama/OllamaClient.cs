using System.Net.Http.Json;
using SCA.Application.Abstractions;
using SCA.Application.Common;
using SCA.Infrastructure.Ollama.Models;
using Microsoft.Extensions.Logging;

namespace SCA.Infrastructure.Ollama;

/// <summary>
/// Handles HTTP communication between the API and Ollama.
/// </summary>
public sealed class OllamaClient(
    HttpClient httpClient,
    OllamaOptions options,
    ILogger<OllamaClient> logger) : IOllamaClient
{
    private const string TagsEndpoint = "/api/tags";
    private const string ChatEndpoint = "/api/chat";

    /// <summary>
    /// Checks whether the configured model exists in Ollama.
    /// </summary>
    public async Task<bool> IsModelReadyAsync(CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync(TagsEndpoint, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            OllamaTagsResponse? result = await response.Content.ReadFromJsonAsync<OllamaTagsResponse>(cancellationToken: cancellationToken);
            bool modelReady = result?.Models?.Any(model =>
                string.Equals(model.Name, options.Model, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(model.Model, options.Model, StringComparison.OrdinalIgnoreCase)) == true;

            if (!modelReady)
            {
                string command = $"ollama pull {options.Model}";
                logger.LogError("Ollama model is missing: {Model}. Install it with: {Command}", options.Model, command);
            }

            return modelReady;
        }
        catch (Exception exception)
        {
            logger.LogWarning(exception, "Ollama status could not be read.");
            return false;
        }
    }

    /// <summary>
    /// Sends the user message to Ollama chat endpoint.
    /// </summary>
    public async Task<string> AskAsync(string message, CancellationToken cancellationToken)
    {
        OllamaChatRequest request = new(
            options.Model,
            false,
            new[]
            {
                new OllamaMessage("system", "Kısa, sade ve Türkçe cevap ver."),
                new OllamaMessage("user", message)
            });

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync(ChatEndpoint, request, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            string error = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new ExternalServiceException($"Ollama returned an error: {(int)response.StatusCode} {error}");
        }

        OllamaChatResponse? result = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(cancellationToken: cancellationToken);
        string? answer = result?.Message?.Content?.Trim();

        if (string.IsNullOrWhiteSpace(answer))
        {
            throw new ExternalServiceException("Ollama response could not be read.");
        }

        logger.LogInformation("Ollama response received.");
        return answer;
    }
}
