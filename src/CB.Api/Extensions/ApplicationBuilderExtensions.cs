using CB.Api.Common.Responses;
using CB.Application.Common;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Serilog;

namespace CB.Api.Extensions;

/// <summary>
/// Keeps the HTTP pipeline small and readable.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds logging, exception handling, authentication and controllers.
    /// </summary>
    public static WebApplication UseApiLayer(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                Exception? exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
                ErrorResponse response = CreateErrorResponse(exception);

                context.Response.StatusCode = response.StatusCode;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(response.Body);
            });
        });

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        return app;
    }

    private static ErrorResponse CreateErrorResponse(Exception? exception)
    {
        if (exception is ValidationException validationException)
        {
            Dictionary<string, string[]> errors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(group => group.Key, group => group.Select(error => error.ErrorMessage).ToArray());

            return new ErrorResponse(StatusCodes.Status400BadRequest, ApiResponse.Fail("Validation failed.", errors));
        }

        (int statusCode, string message) = exception switch
        {
            ExternalServiceException external => (StatusCodes.Status503ServiceUnavailable, external.Message),
            InvalidOperationException invalid => (StatusCodes.Status500InternalServerError, invalid.Message),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected error.")
        };

        return new ErrorResponse(statusCode, ApiResponse.Fail(message));
    }

    private sealed record ErrorResponse(int StatusCode, ApiResponse Body);
}
