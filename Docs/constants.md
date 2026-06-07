# Constants

No global constants file. Constants live in their owning context.

| Constant | Location | Value |
|---|---|---|
| `MessageMaxLength` | `Domain/ChatRules.cs:11` | `500` |
| `CacheDuration` | `Domain/ChatRules.cs:16` | `10 minutes` |
| Section name | `BasicAuthOptions.cs:11` | `"BasicAuth"` |
| Section name | `OllamaOptions.cs:11` | `"Ollama"` |
| Section name | `RedisCacheOptions.cs:11` | `"Redis"` |
| Auth scheme | `BasicAuthenticationHandler.cs:25` | `"Basic"` |
| Cache key prefix | `AskChatCommandHandler.cs:57` | `"cache:chat:"` |
| Ollama endpoints | `OllamaClient.cs:17-18` | `"/api/tags"`, `"/api/chat"` |

## ChatRules

`src/SCA.Domain/ChatRules.cs` — the only dedicated rules class:

```csharp
public static class ChatRules
{
    public const int MessageMaxLength = 500;
    public static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(10);
}
```
