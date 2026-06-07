using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace CB.Application.Behaviors;

/// <summary>
/// Runs FluentValidation before a MediatR request reaches its handler.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <summary>
    /// Stops the request when validation fails.
    /// </summary>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        ValidationContext<TRequest> context = new(request);
        ValidationResult[] validationResults = await Task.WhenAll(validators.Select(x => x.ValidateAsync(context, cancellationToken)));
        List<ValidationFailure> failures = validationResults.SelectMany(x => x.Errors).Where(x => x is not null).ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
