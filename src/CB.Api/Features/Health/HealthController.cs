using CB.Api.Common.Controllers;
using CB.Api.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CB.Api.Features.Health;

/// <summary>
/// Shows that the API process is running.
/// </summary>
[AllowAnonymous]
public sealed class HealthController : ApiControllerBase
{
    /// <summary>
    /// Handles GET /api/health.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse> Get()
    {
        return Ok("API is running.");
    }
}
