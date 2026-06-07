# Mimari

Klasörler ikiye ayrıldı.

```text
src
  SCA.Api
  SCA.Application
  SCA.Domain
  SCA.Infrastructure

tests
  SCA.IntegrationTests
```

Katmanlar:

- `SCA.Api`: Controller, authentication, DTO, response wrapper.
- `SCA.Application`: CQRS, MediatR handler, FluentValidation pipeline.
- `SCA.Domain`: Basit iş kuralları.
- `SCA.Infrastructure`: Redis cache ve Ollama HTTP client.
- `SCA.IntegrationTests`: Endpoint testleri.

Feature örneği:

```text
SCA.Api/Features/Chat
SCA.Application/Features/Chat/AskChat
```
