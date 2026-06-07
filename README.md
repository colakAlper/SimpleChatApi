# SimpleChatApi

.NET 8 Web API that proxies chat messages to a local [Ollama](https://ollama.ai) model with Redis caching.

## Quick Start

```bash
docker compose up -d
```

```bash
curl -X POST http://localhost:8080/api/v1/chat \
  -u admin:admin123 \
  -H "Content-Type: application/json" \
  -d "{\"message\":\"Hello\"}"
```

## Proje Gereksinimleri

Proje kararları ve teknik tercihlerin tamamı: **[Docs/overview.md](Docs/overview.md)**

| Başlık | Karar |
|---|---|
| Asterisk Entegrasyonu | `Answer()` → STT → API → TTS → `Playback()` |
| STT | Whisper / whisper.cpp (ücretsiz, lokal, Türkçe) |
| TTS | Piper TTS (ücretsiz, lokal, hafif, `.wav`) |
| Yapay Zeka | Ollama (ücretsiz, lokal, Docker) |
| Test Süreci | Docker Compose → curl / `.http` dosyaları |
| Teknik Tercihler | .NET 8, Ollama, Redis, Serilog, Basic Auth, CQRS/MediatR, FluentValidation, Docker |

## Dokümantasyon

### Başlangıç

| Doc | Topic |
|---|---|
| [Getting Started](Docs/getting-started.md) | Setup, configuration, curl examples |
| [Start](Docs/start.md) | Program.cs, health gate, Serilog, DI chain |
| [Test](Docs/test.md) | Integration tests, fakes |

### Mimari

| Doc | Topic |
|---|---|
| [Architecture](Docs/architecture.md) | Layers, request flow diagram |
| [CQRS](Docs/cqrs.md) | Command, handler, MediatR |
| [DTO](Docs/dto.md) | Request/response objects |
| [Extensions](Docs/extensions.md) | Extension method classes |
| [Constants](Docs/constants.md) | ChatRules, section names |

### Güvenlik

| Doc | Topic |
|---|---|
| [Basic Auth](Docs/basic-auth.md) | HTTP Basic Authentication |

### API

| Doc | Topic |
|---|---|
| [ApiResponse](Docs/api-response.md) | Single response pattern |
| [Exception](Docs/exception.md) | Global exception handler |
| [Validation](Docs/validation.md) | FluentValidation pipeline |

### Altyapı

| Doc | Topic |
|---|---|
| [Redis](Docs/redis.md) | Cache flow, TTL, health check |
| [Ollama](Docs/ollama.md) | Model integration, health check |
| [Logging](Docs/logging.md) | Serilog, structured logging, sinks |
