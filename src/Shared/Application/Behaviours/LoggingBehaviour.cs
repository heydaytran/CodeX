namespace Application.Behaviours;

public class LoggingBehaviour<TRequest, TResponse>(ILogger<LoggingBehaviour<TRequest, TResponse>> logger) : 
    IPipelineBehavior<TRequest, TResponse>
         where TRequest : notnull
         where TResponse : IErrorOr
{
    private static readonly LogLevel LogLevel = LogLevel.Debug;

    private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger = 
        logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);

        if (_logger.IsEnabled(LogLevel))
        {         
            _logger.Log(LogLevel, "Handling request '{RequestName}'.", requestType.Name);

            var properties = new List<PropertyInfo>(requestType.GetProperties());
            properties.ForEach(p =>
            {
                object? value = p.GetValue(request, null);

                _logger.Log(LogLevel, "Property {Property}: {@Value}", $"{requestType.Name}-{p?.Name}", value?.ToString());
            });
        }

        Stopwatch stopwatch = Stopwatch.StartNew();

        var response = await next();

        stopwatch.Stop();
        
        _logger.LogInformation(
            "Handled '{RequestName}' in {ms}ms. Response error? '{IsError}'. Errors '{Errors}'.",
            requestType.Name,
            stopwatch.ElapsedMilliseconds,
            response.IsError,
            response.IsError ? response.Errors : []);

        return response;
    }
}