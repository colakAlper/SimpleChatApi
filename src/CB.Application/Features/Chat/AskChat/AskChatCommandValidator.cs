using CB.Domain;
using FluentValidation;

namespace CB.Application.Features.Chat.AskChat;

/// <summary>
/// Protects the application layer from invalid chat commands.
/// </summary>
public sealed class AskChatCommandValidator : AbstractValidator<AskChatCommand>
{
    /// <summary>
    /// The message must be present and short enough for this demo.
    /// </summary>
    public AskChatCommandValidator()
    {
        RuleFor(x => x.Message)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(ChatRules.MessageMaxLength).WithMessage($"Message can be at most {ChatRules.MessageMaxLength} characters.");
    }
}
