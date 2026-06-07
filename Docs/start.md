# Start (Program.cs)

`src/SCA.Api/Program.cs` — the entire startup in ~50 lines.

## Flow

1. **Bootstrap Serilog** — console logger before the host is built
2. **`builder.UseProjectSerilog()`** — wires Serilog from `appsettings.json`
3. **`builder.Services.AddProjectLayers(configuration)`** — calls `AddApiLayer()` → `AddApplication()` → `AddInfrastructure()`
4. **Health check gate** — if not in `Testing` environment, runs `HealthCheckService.CheckHealthAsync()`. If Redis ping fails or Ollama model is missing → `Log.Fatal` + throw; API never starts.
5. **`app.UseApiLayer()`** — wires middleware: Serilog request logging → exception handler → auth → controllers

## Key details

- `partial class Program` declaration at bottom enables `WebApplicationFactory<Program>` in tests
- `ASPNETCORE_ENVIRONMENT=Testing` skips health checks (tests don't need real Redis/Ollama)
