using CB.Api.Features.Chat.Models;
using CB.Domain;
using FluentValidation;

namespace CB.Api.Features.Chat.Validators;

/// <summary>
/// Validates the public chat request before it reaches the handler.
/// </summary>
public sealed class ChatRequestValidator : AbstractValidator<ChatRequest>
{
    /// <summary>
    /// Keeps the request small and easy to process.
    /// </summary>
    public ChatRequestValidator()
    {
        RuleFor(x => x.Message)
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(ChatRules.MessageMaxLength).WithMessage($"Message can be at most {ChatRules.MessageMaxLength} characters.");
    }
}
