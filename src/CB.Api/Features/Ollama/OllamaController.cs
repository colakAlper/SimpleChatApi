using CB.Api.Common.Controllers;
using CB.Api.Common.Responses;
using CB.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CB.Api.Features.Ollama;

/// <summary>
/// Checks the Ollama model status.
/// </summary>
[Authorize]
public sealed class OllamaController(IOllamaClient ollamaClient) : ApiControllerBase
{
    /// <summary>
    /// Handles GET /api/ollama/status.
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse>> Status(CancellationToken cancellationToken)
    {
        bool isReady = await ollamaClient.IsModelReadyAsync(cancellationToken);
        return Ok(isReady ? "Ollama model is ready." : "Ollama model is not ready.");
    }
}
