using CB.Api.Common.Controllers;
using CB.Api.Common.Responses;
using CB.Api.Features.Chat.Models;
using CB.Application.Features.Chat.AskChat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CB.Api.Features.Chat;

/// <summary>
/// Receives user messages and returns an answer from Ollama.
/// </summary>
[Authorize]
public sealed class ChatController(ISender sender, ILogger<ChatController> logger) : ApiControllerBase
{
    /// <summary>
    /// Handles POST /api/chat.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> Ask([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        // This log is intentionally simple. It helps during the interview demo.
        logger.LogInformation("API received a message.");

        ChatResponse result = await sender.Send(new AskChatCommand(request.Message!), cancellationToken);
        return Ok(result, "Answer is ready.");
    }
}
