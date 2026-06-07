# Ollama

The model is pre-pulled outside the API. The API sends HTTP requests via `HttpClient`.

## Model pull

```bash
docker compose exec ollama ollama pull llama3.2:1b
```

Docker network address: `http://ollama:11434`

## Files

| File | Role |
|---|---|
| `Application/Abstractions/IOllamaClient.cs` | `AskAsync(message)`, `IsModelReadyAsync()` |
| `Infrastructure/Ollama/OllamaClient.cs` | `POST /api/chat` with `stream: false`. System prompt: *"Kisa, sade ve Turkce cevap ver."* |
| `Infrastructure/Ollama/OllamaOptions.cs` | `BaseUrl`, `Model`, `TimeoutSeconds` (default 120) |
| `Infrastructure/Ollama/Models/*.cs` | DTOs matching Ollama REST API contract |

## Health Check

`src/SCA.Infrastructure/HealthChecks/OllamaHealthCheck.cs` — calls `GET /api/tags` and checks if the configured model exists. If missing, the API refuses to start and logs the required pull command.
