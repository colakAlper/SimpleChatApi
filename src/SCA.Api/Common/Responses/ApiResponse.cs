namespace SCA.Api.Common.Responses;

/// <summary>
/// A small and consistent API response model used by every endpoint.
/// </summary>
/// <param name="Success">Shows whether the request completed successfully.</param>
/// <param name="Message">A short human-readable result message.</param>
/// <param name="Data">Optional response data. It is null when the endpoint only returns a message.</param>
public sealed record ApiResponse(bool Success, string Message, object? Data = null)
{
    /// <summary>
    /// Creates a successful response.
    /// </summary>
    public static ApiResponse Ok(string message, object? data = null)
        => new(true, message, data);

    /// <summary>
    /// Creates a failed response.
    /// </summary>
    public static ApiResponse Fail(string message, object? data = null)
        => new(false, message, data);
}
