# Validation ve Authentication

## Validation

Sadece FluentValidation kullanılır. DataAnnotation yoktur.

- `ChatRequest` sadece DTO'dur.
- `ChatRequestValidator` HTTP request'i kontrol eder.
- `AskChatCommandValidator` application katmanını korur.

## Authentication

Basic Authentication kullanılır.

```bash
-u admin:admin123
```

Authentication ve validation cevapları aynı `ApiResponse` modeliyle döner.
