# Exception Handling

Global exception handler in `src/SCA.Api/Extensions/ApplicationBuilderExtensions.cs:22-34`.

Every exception returns the `ApiResponse` shape — no stack traces leak to clients.

## Exception → HTTP mapping

| Exception | HTTP | Response |
|---|---|---|
| `FluentValidation.ValidationException` | 400 | `ApiResponse.Fail` with grouped property errors |
| `ExternalServiceException` | 503 | `ApiResponse.Fail` with exception message |
| `InvalidOperationException` | 500 | `ApiResponse.Fail` with exception message |
| Anything else | 500 | `ApiResponse.Fail("Unexpected error.")` |

## ExternalServiceException

`src/SCA.Application/Common/ExternalServiceException.cs`

```csharp
public sealed class ExternalServiceException(string message) : Exception(message);
```

Thrown when Redis or Ollama cannot produce a valid result. Always maps to HTTP 503.

## Startup exceptions

`src/SCA.Api/Program.cs:9-46` has a top-level `try/catch` that logs `Log.Fatal` and rethrows on startup failures (e.g., missing config, failed health checks).
