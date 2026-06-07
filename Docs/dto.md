# DTO

The project keeps DTOs minimal — no mapping layer.

## Request

`src/SCA.Application/Features/Chat/AskChat/AskChatCommand.cs`

```csharp
public sealed record AskChatCommand(string Message) : IRequest<ChatResponse>;
```

Serves dual purpose: both the MediatR command and the JSON request body deserialized by ASP.NET. The controller binds it as `[FromBody]` and sends directly via `ISender`.

## Response

`src/SCA.Application/Features/Chat/AskChat/ChatResponse.cs`

```csharp
public sealed record ChatResponse(string Response);
```

Returned by the handler, wrapped in `ApiResponse` by the controller's `Ok()` helper.

## Ollama Models

Infrastructure-level DTOs matching the Ollama REST API contract live under `Infrastructure/Ollama/Models/`: `OllamaChatRequest`, `OllamaChatResponse`, `OllamaMessage`, `OllamaModel`, `OllamaTagsResponse`.
