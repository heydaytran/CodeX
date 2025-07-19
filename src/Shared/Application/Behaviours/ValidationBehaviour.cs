namespace Application.Behaviours;

public class ValidationBehaviour<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationBehaviour<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
     where TResponse : IErrorOr
{
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators ?? throw new ArgumentNullException(nameof(validators));
    private readonly ILogger<ValidationBehaviour<TRequest, TResponse>> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next();
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count != 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count == 0)
        {
            return await next();
        }

        failures.ForEach(f => 
            _logger.LogDebug(
                "{ValidationPropertyName} failed validation. Code {ValidationErrorCode}, message {ValidationErrorMessage}.",
                f.PropertyName,
                f.ErrorCode,
                f.ErrorMessage));

        return TryCreateErrorResponseFromErrors(failures, out var errorResponse)
            ? errorResponse
            : throw new ValidationException(failures);
    }

    private static bool TryCreateErrorResponseFromErrors(List<ValidationFailure> validationFailures, out TResponse errorResponse)
    {
        List<Error> errors = validationFailures
            .ConvertAll(x => Error.Validation(code: x.PropertyName, description: x.ErrorMessage));

        try
        {
            errorResponse = (TResponse)(dynamic)errors;
            return true;
        }
        catch
        {
            errorResponse = default!;
            return false;
        }
    }
}
