# ApiResponse (Single Response Pattern)

Every endpoint returns the same JSON shape — success and failure use one model.

```json
{ "success": true,  "message": "Answer is ready.",  "data": { "response": "..." } }
{ "success": true,  "message": "API is running.",    "data": null }
{ "success": false, "message": "Validation failed.", "data": { "Message": ["..."] } }
```

## ApiResponse

`src/SCA.Api/Common/Responses/ApiResponse.cs`

`sealed record ApiResponse(bool Success, string Message, object? Data)`

Static factories:
- `ApiResponse.Ok(message, data?)` → `success: true`
- `ApiResponse.Fail(message, data?)` → `success: false`

## ApiControllerBase

`src/SCA.Api/Common/Controllers/ApiControllerBase.cs`

Base class for all controllers. Sets `[Route("api/v1/[controller]")]` and `[ApiController]`.

Provides `Ok()` overloads that wrap results in `ApiResponse`:

```csharp
return Ok("API is running.");               // message only
return Ok(result, "Answer is ready.");      // data + message
```
