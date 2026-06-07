using SCA.Api.Common.Controllers;
using SCA.Api.Common.Responses;
using SCA.Application.Features.Chat.AskChat;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SCA.Api.Controllers;

/// <summary>
/// Receives user messages and returns an answer from Ollama.
/// </summary>
[Authorize]
public sealed class ChatController(ISender sender, ILogger<ChatController> logger) : ApiControllerBase
{
    /// <summary>
    /// Handles POST /api/v1/chat.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse>> Ask([FromBody] AskChatCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("API received a message.");

        ChatResponse result = await sender.Send(request, cancellationToken);
        return Ok(result, "Answer is ready.");
    }
}
