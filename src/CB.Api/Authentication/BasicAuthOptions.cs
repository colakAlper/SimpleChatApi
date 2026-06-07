namespace CB.Api.Authentication;

/// <summary>
/// Basic authentication settings from appsettings.json.
/// </summary>
public sealed class BasicAuthOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "BasicAuth";

    /// <summary>
    /// API username.
    /// </summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>
    /// API password.
    /// </summary>
    public string Password { get; init; } = string.Empty;

    /// <summary>
    /// Stops the application early when authentication settings are missing.
    /// </summary>
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            throw new InvalidOperationException("BasicAuth:Username cannot be empty.");
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            throw new InvalidOperationException("BasicAuth:Password cannot be empty.");
        }
    }
}
