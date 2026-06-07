# Validation (FluentValidation)

Two validation layers. No `[Required]` attributes anywhere.

## Level 1 — DTO (model binding)

`src/SCA.Api/Extensions/ServiceCollectionExtensions.cs:39-53`

Configures `ApiBehaviorOptions.InvalidModelStateResponseFactory` to return `ApiResponse.Fail("Validation failed.", errors)` on model binding failure.

## Level 2 — Application (MediatR pipeline)

`src/SCA.Application/Behaviors/ValidationBehavior.cs`

`ValidationBehavior<TRequest, TResponse>` runs before every handler. Collects all `IValidator<TRequest>` instances, executes in parallel, throws `FluentValidation.ValidationException` on failure → caught by global exception handler → 400.

## Validator

`src/SCA.Application/Features/Chat/AskChat/AskChatCommandValidator.cs`

```csharp
RuleFor(x => x.Message)
    .NotEmpty().WithMessage("Message cannot be empty.")
    .MaximumLength(ChatRules.MessageMaxLength).WithMessage($"...");
```

## Registration

`src/SCA.Application/DependencyInjection.cs:19-20`

```csharp
services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```
