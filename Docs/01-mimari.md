# Mimari

Klasörler ikiye ayrıldı.

```text
src
  CB.Api
  CB.Application
  CB.Domain
  CB.Infrastructure

tests
  CB.IntegrationTests
```

Katmanlar:

- `CB.Api`: Controller, authentication, DTO, response wrapper.
- `CB.Application`: CQRS, MediatR handler, FluentValidation pipeline.
- `CB.Domain`: Basit iş kuralları.
- `CB.Infrastructure`: Redis cache ve Ollama HTTP client.
- `CB.IntegrationTests`: Endpoint testleri.

Feature örneği:

```text
CB.Api/Features/Chat
CB.Application/Features/Chat/AskChat
```
