# Basic Auth

Trivial HTTP Basic Authentication. No JWT, no Identity.

**Credentials:** `admin` / `admin123` (configurable via `appsettings.json` or env vars).

## Files

| File | Role |
|---|---|
| `Authentication/BasicAuthOptions.cs` | POCO from `"BasicAuth"` config section. `Validate()` called at startup — throws if empty. |
| `Authentication/BasicAuthenticationHandler.cs` | Decodes `Authorization: Basic <base64>`, splits on `:`, compares to configured credentials. Returns `ApiResponse.Fail(...)` for 401/403. |

## Behavior

- `/api/v1/health` — `[AllowAnonymous]`, no auth needed
- All other endpoints — `[Authorize]`
- Failed auth returns the same `ApiResponse` shape as everything else

## Registered in

`src/SCA.Api/Extensions/ServiceCollectionExtensions.cs:25-29`
