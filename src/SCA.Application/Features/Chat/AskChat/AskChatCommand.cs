using MediatR;

namespace SCA.Application.Features.Chat.AskChat;

/// <summary>
/// CQRS command used to ask a chat question.
/// </summary>
/// <param name="Message">User message.</param>
public sealed record AskChatCommand(string Message) : IRequest<ChatResponse>;
