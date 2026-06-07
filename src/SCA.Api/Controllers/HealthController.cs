using SCA.Api.Common.Controllers;
using SCA.Api.Common.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SCA.Api.Controllers;

/// <summary>
/// Shows that the API process is running.
/// </summary>
[AllowAnonymous]
public sealed class HealthController : ApiControllerBase
{
    /// <summary>
    /// Handles GET /api/v1/health.
    /// </summary>
    [HttpGet]
    public ActionResult<ApiResponse> Get()
    {
        return Ok("API is running.");
    }
}
