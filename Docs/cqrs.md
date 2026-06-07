# CQRS

Single feature: `AskChat`. Uses [MediatR](https://github.com/jbogard/MediatR).

## Files

| File | Role |
|---|---|
| `Application/Features/Chat/AskChat/AskChatCommand.cs` | `record AskChatCommand(string Message) : IRequest<ChatResponse>` |
| `Application/Features/Chat/AskChat/AskChatCommandHandler.cs` | Implements `IRequestHandler<AskChatCommand, ChatResponse>`. Checks Redis cache → calls Ollama → stores response. |
| `Application/Features/Chat/AskChat/ChatResponse.cs` | `record ChatResponse(string Response)` — returned to the controller. |

## Flow

```csharp
// Controller sends the command:
ChatResponse result = await sender.Send(request, cancellationToken);
return Ok(result, "Answer is ready.");
```

The handler (`AskChatCommandHandler.cs:22-46`):
1. Normalize message → SHA-256 hash → cache key `cache:chat:{hash}`
2. Check Redis → hit returns immediately
3. Miss: call Ollama → validate response → store in Redis (10 min TTL)

## Registration

`src/SCA.Application/DependencyInjection.cs:18` — `services.AddMediatR(...)` scans the assembly.
