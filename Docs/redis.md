# Redis Cache

Same question → same answer from cache for 10 minutes.

## Files

| File | Role |
|---|---|
| `Application/Abstractions/IRedisCache.cs` | `GetStringAsync`, `SetStringAsync` |
| `Infrastructure/Cache/RedisCache.cs` | Wraps `IDistributedCache` (StackExchange.Redis) |
| `Infrastructure/Cache/RedisCacheOptions.cs` | `ConnectionString`, `InstanceName` |

## Cache flow

`src/SCA.Application/Features/Chat/AskChat/AskChatCommandHandler.cs:51-58`

1. Normalize message: trim + lowercase
2. Compute `SHA-256` hash → key `cache:chat:{hash}`
3. `GetStringAsync(key)` → hit: return immediately
4. Miss: call Ollama → `SetStringAsync(key, answer, 10 min)`

## TTL

`src/SCA.Domain/ChatRules.cs:16`

```csharp
public static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
```

## Health Check

`src/SCA.Infrastructure/HealthChecks/RedisHealthCheck.cs` — pings Redis at startup. If unreachable the API refuses to start.
