using SCA.Api.Common.Controllers;
using SCA.Api.Common.Responses;
using SCA.Application.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SCA.Api.Controllers;

/// <summary>
/// Checks the Ollama model status.
/// </summary>
[Authorize]
public sealed class OllamaController(IOllamaClient ollamaClient) : ApiControllerBase
{
    /// <summary>
    /// Handles GET /api/v1/ollama/status.
    /// </summary>
    [HttpGet("status")]
    public async Task<ActionResult<ApiResponse>> Status(CancellationToken cancellationToken)
    {
        bool isReady = await ollamaClient.IsModelReadyAsync(cancellationToken);
        return Ok(isReady ? "Ollama model is ready." : "Ollama model is not ready.");
    }
}
