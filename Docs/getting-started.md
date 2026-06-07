# Getting Started

## Setup

```bash
cp .env.example .env
docker compose up -d redis ollama
docker compose exec ollama ollama pull llama3.2:1b
docker compose up --build api
```

## Verify

```bash
curl http://localhost:8080/api/v1/health
curl http://localhost:8080/api/v1/ollama/status -u admin:admin123
curl -X POST http://localhost:8080/api/v1/chat \
  -u admin:admin123 \
  -H "Content-Type: application/json" \
  -d "{\"message\":\"Hello\"}"
```

Or open `http/chatbot.http` in VS Code / Rider (REST Client).

## Configuration

### appsettings.json

```json
{
  "BasicAuth": { "Username": "admin", "Password": "admin123" },
  "Ollama":    { "BaseUrl": "http://localhost:11434", "Model": "llama3.2:1b", "TimeoutSeconds": 120 },
  "Redis":     { "ConnectionString": "localhost:6379,abortConnect=false", "InstanceName": "cb:" }
}
```

### .env

```
BASIC_AUTH_USERNAME=admin
BASIC_AUTH_PASSWORD=admin123
OLLAMA_MODEL=llama3.2:1b
```

### Environment variables (docker compose)

```yaml
BasicAuth__Username: ${BASIC_AUTH_USERNAME:-admin}
BasicAuth__Password: ${BASIC_AUTH_PASSWORD:-admin123}
Ollama__BaseUrl:     http://ollama:11434
Ollama__Model:       ${OLLAMA_MODEL:-llama3.2:1b}
Redis__ConnectionString: redis:6379,abortConnect=false
```
