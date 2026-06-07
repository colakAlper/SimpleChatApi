using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using SCA.Api.Common.Responses;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace SCA.Api.Authentication;

/// <summary>
/// Very small Basic authentication handler for the demo API.
/// </summary>
public sealed class BasicAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    BasicAuthOptions basicAuthOptions)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>
    /// Authentication scheme name used by the API.
    /// </summary>
    public const string SchemeName = "Basic";

    /// <summary>
    /// Reads the Authorization header and validates username/password.
    /// </summary>
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out AuthenticationHeaderValue? headerValue) ||
            !string.Equals(headerValue.Scheme, SchemeName, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(headerValue.Parameter))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid Authorization header."));
        }

        string rawCredentials;

        try
        {
            rawCredentials = Encoding.UTF8.GetString(Convert.FromBase64String(headerValue.Parameter));
        }
        catch (FormatException)
        {
            return Task.FromResult(AuthenticateResult.Fail("Authorization header is not valid base64."));
        }

        string[] parts = rawCredentials.Split(':', 2);

        if (parts.Length != 2 ||
            parts[0] != basicAuthOptions.Username ||
            parts[1] != basicAuthOptions.Password)
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid username or password."));
        }

        ClaimsIdentity identity = new(new[] { new Claim(ClaimTypes.Name, basicAuthOptions.Username) }, SchemeName);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    /// <summary>
    /// Returns the same response shape when authentication is missing or invalid.
    /// </summary>
    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status401Unauthorized;
        Response.ContentType = "application/json";
        Response.Headers["WWW-Authenticate"] = "Basic realm=\"SCA\"";

        await Response.WriteAsJsonAsync(ApiResponse.Fail("Authentication is required."));
    }

    /// <summary>
    /// Returns the same response shape when the user is authenticated but not allowed.
    /// </summary>
    protected override async Task HandleForbiddenAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        Response.ContentType = "application/json";

        await Response.WriteAsJsonAsync(ApiResponse.Fail("Access is forbidden."));
    }
}
