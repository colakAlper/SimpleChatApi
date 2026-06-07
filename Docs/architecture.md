# Architecture

## Layer Diagram

```mermaid
graph TD
    Client[HTTP Client] -->|Basic Auth| Api[SCA.Api]
    Api -->|MediatR| App[SCA.Application]
    App -->|Rules| Domain[SCA.Domain]
    App -->|Abstractions| Infra[SCA.Infrastructure]
    Infra -->|HttpClient| Ollama[Ollama]
    Infra -->|StackExchange.Redis| Redis[Redis]
    Api --> Infra

    subgraph API Layer
        Api
    end
    subgraph Application Layer
        App
    end
    subgraph Domain Layer
        Domain
    end
    subgraph Infrastructure Layer
        Infra
    end
```

`SCA.Api` references both `SCA.Application` (handler contracts) and `SCA.Infrastructure` (health check types for DI registration).

## Request Flow — Step by Step

```mermaid
sequenceDiagram
    participant C as Client
    participant MW as Middleware
    participant CT as ChatController
    participant VB as ValidationBehavior
    participant H as Handler
    participant R as Redis
    participant O as Ollama

    C->>MW: POST /api/v1/chat<br/>Authorization: Basic <base64><br/>{"message":"Hello"}

    rect rgb(240, 248, 255)
        Note over MW: Authentication
        MW->>MW: BasicAuthenticationHandler<br/>decode header, check credentials
        alt missing/invalid
            MW-->>C: 401 ApiResponse.Fail("Authentication is required.")
        end
    end

    MW->>CT: ChatController.Ask(AskChatCommand)

    rect rgb(255, 248, 240)
        Note over CT: Model Binding
        CT->>CT: Deserialize JSON body<br/>→ AskChatCommand("Hello")
        CT->>CT: ApiBehaviorOptions check<br/>DTO-level validation
        alt DTO invalid
            CT-->>C: 400 ApiResponse.Fail("Validation failed.", errors)
        end
    end

    CT->>VB: sender.Send(command)

    rect rgb(240, 255, 240)
        Note over VB: MediatR Pipeline
        VB->>VB: AskChatCommandValidator<br/>not empty? ≤ 500 chars?
        alt application validation fails
            VB->>VB: throw ValidationException
            Note over C,VB: GlobalExceptionHandler → 400
        end
    end

    VB->>H: AskChatCommandHandler.Handle()

    rect rgb(255, 255, 240)
        Note over H: Core Logic
        H->>H: Normalize message<br/>→ SHA-256 hash<br/>→ key = "cache:chat:{hash}"
        H->>R: GetStringAsync(key)
        alt cache hit
            R-->>H: cached response
            H-->>C: 200 ChatResponse
        end
        H->>O: AskAsync("Hello")
        O-->>H: model answer
        alt empty response
            H->>H: throw ExternalServiceException
            Note over C,H: GlobalExceptionHandler → 503
        end
        H->>R: SetStringAsync(key, answer, 10min)
        H-->>C: 200 ChatResponse
    end

    Note over C: {"success":true,"message":"Answer is ready.","data":{"response":"..."}}
```

## Layer Dependencies

| Layer       | Depends on                 | Contains |
|------------|----------------------------|----------|
| `SCA.Api` | Application, Infrastructure | Controllers, Auth, ApiResponse, Extensions, Program.cs |
| `SCA.Application` | Domain | CQRS handlers, ValidationBehavior, IOllamaClient, IRedisCache |
| `SCA.Domain` | (none) | ChatRules |
| `SCA.Infrastructure` | Application | RedisCache, OllamaClient, HealthChecks, Options |
| `SCA.IntegrationTests` | Api | CustomWebApplicationFactory, Fakes, Endpoint tests |

## Feature Folder Convention

```
SCA.Application/Features/Chat/AskChat/
  AskChatCommand.cs         # Command (also request DTO)
  AskChatCommandHandler.cs  # Handler (cache → ollama → cache)
  AskChatCommandValidator.cs# FluentValidation rules
  ChatResponse.cs           # Return DTO
```

Same feature is exposed in the API layer:

```
SCA.Api/Controllers/
  ChatController.cs   # POST /api/v1/chat → sends command
  HealthController.cs # GET /api/v1/health
  OllamaController.cs # GET /api/v1/ollama/status
```

## Key Files Reference

| File | Lines | Responsibility |
|---|---|---|
| `Api/Program.cs` | 55 | Startup, health check gate, Serilog bootstrap |
| `Api/Common/Responses/ApiResponse.cs` | 22 | Unified `{ success, message, data }` model |
| `Api/Common/Controllers/ApiControllerBase.cs` | 24 | Route prefix, Ok helpers |
| `Api/Authentication/BasicAuthenticationHandler.cs` | 93 | Basic auth with ApiResponse on fail |
| `Api/Extensions/ServiceCollectionExtensions.cs` | 74 | DI: auth, controllers, validation, health checks |
| `Api/Extensions/ApplicationBuilderExtensions.cs` | 65 | Middleware: logging, exception handler, auth |
| `Api/Extensions/WebApplicationBuilderExtensions.cs` | 37 | Serilog + layer registration chain |
| `Application/DependencyInjection.cs` | 24 | MediatR, validators, pipeline |
| `Application/Behaviors/ValidationBehavior.cs` | 35 | FluentValidation pipeline behavior |
| `Application/Features/Chat/AskChat/AskChatCommandHandler.cs` | 59 | Core logic: cache → ollama → cache |
| `Application/Features/Chat/AskChat/AskChatCommandValidator.cs` | 21 | Not empty, max 500 chars |
| `Application/Common/ExternalServiceException.cs` | 6 | Custom exception → 503 |
| `Domain/ChatRules.cs` | 17 | MessageMaxLength, CacheDuration |
| `Infrastructure/Cache/RedisCache.cs` | 31 | IDistributedCache wrapper |
| `Infrastructure/Ollama/OllamaClient.cs` | 86 | HttpClient → POST /api/chat |
| `Infrastructure/HealthChecks/RedisHealthCheck.cs` | 20 | Startup gate: Redis ping |
| `Infrastructure/HealthChecks/OllamaHealthCheck.cs` | 21 | Startup gate: model existence |
| `tests/SCA.IntegrationTests/CustomWebApplicationFactory.cs` | 42 | Test host with fakes |
| `tests/SCA.IntegrationTests/Features/Chat/ChatEndpointTests.cs` | 110 | 5 integration tests |
