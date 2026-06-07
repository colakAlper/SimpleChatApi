namespace CB.Api.Features.Chat.Models;

/// <summary>
/// Request body for the chat endpoint.
/// </summary>
public sealed class ChatRequest
{
    /// <summary>
    /// User message sent to the API.
    /// </summary>
    public string? Message { get; init; }
}
