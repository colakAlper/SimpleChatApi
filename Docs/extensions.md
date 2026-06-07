# Extensions

Three extension classes keep `Program.cs` minimal (~50 lines).

`src/SCA.Api/Extensions/`

| File | Key Method | Role |
|---|---|---|
| `WebApplicationBuilderExtensions.cs` | `UseProjectSerilog()` | Serilog wiring from appsettings |
| | `AddProjectLayers()` | Chains `AddApiLayer()` → `AddApplication()` → `AddInfrastructure()` |
| `ServiceCollectionExtensions.cs` | `AddApiLayer()` | Controllers, auth, validation, health checks |
| `ApplicationBuilderExtensions.cs` | `UseApiLayer()` | Request logging, exception handler, auth middleware, controller mapping |

## DI chain

`WebApplicationBuilderExtensions.cs:28-36`

```csharp
services
    .AddApiLayer(configuration)       // Auth, controllers, DTO validation, health
    .AddApplication()                 // MediatR, validators, pipeline behavior
    .AddInfrastructure(configuration); // Redis, Ollama HttpClient, health checks
```

Each layer has its own `DependencyInjection` class:
- `src/SCA.Application/DependencyInjection.cs`
- `src/SCA.Infrastructure/DependencyInjection.cs`
