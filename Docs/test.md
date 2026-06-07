# Test Infrastructure

Integration tests in `tests/SCA.IntegrationTests/`.

## Files

| File | Role |
|---|---|
| `CustomWebApplicationFactory.cs` | `WebApplicationFactory<Program>`. Sets `ASPNETCORE_ENVIRONMENT=Testing`. Replaces `IOllamaClient` and `IRedisCache` with fake implementations. |
| `Fakes/CountingOllamaClient.cs` | Returns `"Test answer: {message}"`. Tracks call count via `Interlocked.Increment`. |
| `Fakes/InMemoryRedisCache.cs` | Uses `MemoryCache` with TTL support. |
| `Features/Chat/ChatEndpointTests.cs` | 5 xUnit integration tests |

## Tests

| Test | What it verifies |
|---|---|
| `Chat_WithValidRequest_ReturnsStandardResponse` | 200 + `success: true` + expected answer |
| `Chat_WithSameQuestion_UsesCache` | Second identical request does not call Ollama again |
| `Chat_WithInvalidRequest_Returns400` | Empty message → 400 + `success: false` |
| `Chat_WithoutAuthentication_Returns401` | Missing auth → 401 + standard error shape |
| `Health_ReturnsOkWithoutAuthentication` | Health endpoint works anonymously |

## Run

```bash
dotnet test SCA.sln
```
