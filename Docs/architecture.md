# Architecture

Four layers, top-down: Api → Application → Domain → Infrastructure.

```
┌──────────────────────────────────────────────────┐
│                     SCA.Api                       │
│  Controllers  │  Basic Auth  │  ApiResponse       │
│  Extensions   │  Program.cs                       │
└────────┬───────────────────────────┬──────────────┘
         │ MediatR                   │ HealthChecks
         ▼                           ▼
┌─────────────────────┐   ┌─────────────────────────┐
│  SCA.Application     │   │  SCA.Infrastructure      │
│                       │   │                          │
│  CQRS Handlers        │◄──│  RedisCache              │
│  ValidationBehavior   │   │  OllamaClient            │
│  IOllamaClient        │   │  HealthChecks            │
│  IRedisCache          │   │                          │
└───────┬───────────────┘   └──────────────────────────┘
        │
        ▼
┌─────────────────────┐
│  SCA.Domain          │
│  ChatRules           │
└──────────────────────┘
```

| Layer | Depends On | Contains |
|---|---|---|
| `SCA.Api` | Application, Infrastructure | Controllers, BasicAuthHandler, ApiResponse, Extensions, Program.cs |
| `SCA.Application` | Domain | CQRS handlers, ValidationBehavior, interfaces (IOllamaClient, IRedisCache) |
| `SCA.Domain` | — | ChatRules (max length, cache TTL) |
| `SCA.Infrastructure` | Application | RedisCache, OllamaClient, HealthChecks, Options |
| `SCA.IntegrationTests` | Api | CustomWebApplicationFactory, Fakes, Endpoint tests |

---

## Request Flow

A `POST /api/v1/chat` request passes through these steps:

```
  1. MIDDLEWARE ── BasicAuthenticationHandler decodes Authorization header
     │
     │  fail → 401  {"success":false, "message":"Authentication is required."}
     │
  2. CONTROLLER ── ChatController.Ask()
     │                binds JSON body → AskChatCommand
     │                runs DTO-level validation (model binding)
     │
     │  fail → 400  {"success":false, "message":"Validation failed.", ...}
     │
  3. MEDIATR PIPELINE ── ValidationBehavior
     │                       runs AskChatCommandValidator
     │                       (not empty? ≤ 500 chars?)
     │
     │  fail → throws ValidationException → 400
     │
  4. HANDLER ── AskChatCommandHandler.Handle()
     │             │
     │             ├─► redis.GetStringAsync("cache:chat:{sha256}")
     │             │     hit → return cached   ───────────────────── 200
     │             │
     │             ├─► ollamaClient.AskAsync(message)
     │             │     │
     │             │     fail → throws ExternalServiceException → 503
     │             │
     │             └─► redis.SetStringAsync(key, answer, 10 min TTL)
     │                                                              ── 200
     │
  5. RESPONSE ── ApiControllerBase wraps ChatResponse in ApiResponse.Ok()
       →  {"success":true, "message":"Answer is ready.", "data":{"response":"..."}}
```

---

## Feature Folder Convention

Each feature groups its command, handler, validator, and response in one folder:

```
Application/Features/Chat/AskChat/
  AskChatCommand.cs          # record AskChatCommand(string Message) : IRequest<ChatResponse>
  AskChatCommandHandler.cs   # cache check → Ollama call → cache write
  AskChatCommandValidator.cs # not empty, max 500 chars
  ChatResponse.cs            # record ChatResponse(string Response)
```

The same feature is exposed in the API layer:

```
Api/Controllers/
  ChatController.cs    # POST /api/v1/chat  → sender.Send(command)
  HealthController.cs  # GET  /api/v1/health
  OllamaController.cs  # GET  /api/v1/ollama/status
```
