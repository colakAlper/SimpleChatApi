namespace CB.Application.Common;

/// <summary>
/// Used when an external dependency cannot return a valid result.
/// </summary>
public sealed class ExternalServiceException(string message) : Exception(message);
