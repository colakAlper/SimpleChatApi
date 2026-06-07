using System.Net.Http.Json;
using CB.Application.Abstractions;
using CB.Application.Common;
using CB.Infrastructure.Ollama.Models;
using Microsoft.Extensions.Logging;

namespace CB.Infrastructure.Ollama;

/// <summary>
/// Handles HTTP communication between the API and Ollama.
/// </summary>
public sealed class OllamaClient(
    HttpClient httpClient,
    OllamaOptions options,
    ILogger<OllamaClient> logger) : IOllamaClient
{
    /// <summary>
    /// Checks whether the configured model exists in Ollama.
    /// </summary>
    public async Task<bool> IsModelReadyAsync(CancellationToken cancellationToken)
    {
        try
        {
            using HttpResponseMessage response = await httpClient.GetAsync("/api/tags", cancellationToken); // TODO: Const endpointler olmalı.
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            OllamaTagsResponse? result = await response.Content.ReadFromJsonAsync<OllamaTagsResponse>(cancellationToken: cancellationToken);
            bool modelReady = result?.Models?.Any(model =>
                string.Equals(model.Name, options.Model, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(model.Model, options.Model, StringComparison.OrdinalIgnoreCase)) == true;

            // TODO: ModelReady ne demek, model hiç yok mu demek? Docker'dan kurulum gerekiyorsa net olarak uyarı fırlatılmalıdır.
            if (!modelReady)
            {
                logger.LogWarning("Ollama is running but the model is missing: {Model}", options.Model);
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

        using HttpResponseMessage response = await httpClient.PostAsJsonAsync("/api/chat", request, cancellationToken); // TODO: Const endpointler olmalı.

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
