# Serilog Logging

SQL-style structured logging via Serilog. Writes to Console + rolling file through async sink.

## Packages

`src/SCA.Api/SCA.Api.csproj`

| Package | Version | Role |
|---|---|---|
| `Serilog.AspNetCore` | 8.0.3 | Replaces `Microsoft.Extensions.Logging` |
| `Serilog.Settings.Configuration` | 8.0.4 | Reads config from `appsettings.json` |
| `Serilog.Sinks.Async` | 1.5.0 | Non-blocking writes |
| `Serilog.Sinks.Console` | 5.0.1 | Console output |
| `Serilog.Sinks.File` | 5.0.0 | Rolling file output |

Other layers (`Application`, `Infrastructure`) depend only on `Microsoft.Extensions.Logging.Abstractions` — Serilog resolves at runtime via the host.

## Configuration

`src/SCA.Api/appsettings.json:15-41`

```json
"Serilog": {
    "Using": [
      "Serilog.Sinks.Console",
      "Serilog.Sinks.File",
      "Serilog.Sinks.Async"
    ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "Enrich": [ "FromLogContext" ],
    "WriteTo": [
      {
        "Name": "Async",
        "Args": {
          "configure": [
            { "Name": "Console" },
            {
              "Name": "File",
              "Args": {
                "path": "logs/cb-api-.log",
                "rollingInterval": "Day",
                "retainedFileCountLimit": 7
              }
            }
          ]
        }
      }
    ]
  }
```

| Setting | Value | Notes |
|---|---|---|
| Minimum level | `Information` | `Microsoft.*` and `System.*` suppressed to `Warning` |
| Sinks | Console + File | Both wrapped by Async |
| File path | `logs/cb-api-.log` | One file per day |
| Retention | 7 days | Older files auto-deleted |
| Enricher | `FromLogContext` | Enables `LogContext.PushProperty()` |

Development overrides `MinimumLevel.Default` to `Information` in `appsettings.Development.json`.

## Startup bootstrap

`src/SCA.Api/Program.cs:5-7`

```csharp
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();
```

A console-only logger created **before** the host is built. Captures any failure during configuration binding or DI setup.

`src/SCA.Api/Extensions/WebApplicationBuilderExtensions.cs:14-19`

```csharp
builder.Host.UseSerilog((context, _, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(context.Configuration);
});
```

Replaces the default .NET logger with Serilog, reading the full configuration from `appsettings.json`.

## Request logging

`src/SCA.Api/Extensions/ApplicationBuilderExtensions.cs:12`

```csharp
app.UseSerilogRequestLogging();
```

Logs every HTTP request with method, path, status code, and elapsed time — all as structured properties.

## Shutdown

`src/SCA.Api/Program.cs:44-49`

```csharp
catch (Exception exception)
{
    Log.Fatal(exception, "Application could not start.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}
```

`Log.CloseAndFlush()` drains buffered async logs before exit. The `catch` block surfaces any startup failure as fatal.

## Log points

| Layer | Class | Level | Messages |
|---|---|---|---|
| API | `ChatController` | `Information` | "API received a message." |
| API | `Program.cs` (startup) | `Fatal` | Health check failure, startup exception |
| Application | `AskChatCommandHandler` | `Information` | Cache hit, Ollama call, cache save |
| Infrastructure | `OllamaClient` | `Information` | "Ollama response received." |
| | | `Warning` | Ollama status check failure |
| | | `Error` | Model not found (with install command) |
| Infrastructure | `DependencyInjection` | `Warning` | Redis connection failure |

## Usage pattern

All classes depend on `ILogger<T>` via primary constructor injection:

```csharp
public sealed class AskChatCommandHandler(
    IRedisCache redisCache,
    IOllamaClient ollamaClient,
    ILogger<AskChatCommandHandler> logger) : IRequestHandler<AskChatCommand, ChatResponse>
```

For cases without a class (e.g. factory delegates), use `ILoggerFactory`:

```csharp
ILogger logger = serviceProvider.GetRequiredService<ILoggerFactory>()
    .CreateLogger("RedisConnection");
```

## Structured logging

All messages use Serilog's named property syntax (`{Property}`) rather than string interpolation. This keeps log events queryable by property name in sinks like Seq or Elasticsearch.

```csharp
// Good — property is indexed
logger.LogError("Ollama model is missing: {Model}. Install it with: {Command}", options.Model, command);

// Avoid
logger.LogError($"Ollama model is missing: {options.Model}.");
```
