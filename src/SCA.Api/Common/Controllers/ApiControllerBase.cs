using SCA.Api.Common.Responses;
using Microsoft.AspNetCore.Mvc;

namespace SCA.Api.Common.Controllers;

/// <summary>
/// Keeps routing and response helpers in one place.
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public abstract class ApiControllerBase : ControllerBase
{
    /// <summary>
    /// Returns a simple successful response without data.
    /// </summary>
    protected new ActionResult<ApiResponse> Ok(string message)
        => base.Ok(ApiResponse.Ok(message));

    /// <summary>
    /// Returns a successful response with data.
    /// </summary>
    protected ActionResult<ApiResponse> Ok(object data, string message = "OK")
        => base.Ok(ApiResponse.Ok(message, data));
}
