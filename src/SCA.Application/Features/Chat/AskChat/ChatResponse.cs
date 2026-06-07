namespace SCA.Application.Features.Chat.AskChat;

/// <summary>
/// Chat answer returned by the application layer.
/// </summary>
/// <param name="Response">Generated answer text.</param>
public sealed record ChatResponse(string Response);
