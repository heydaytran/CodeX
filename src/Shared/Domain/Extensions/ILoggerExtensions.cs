namespace Domain.Extensions;

public static class ILoggerExtensions
{
    public static void LogError<T>(this ILogger<T> logger, List<Error> errors)
    {
        logger.LogError("{Error}", errors.ToLogString());
    }

    public static void LogError<T>(this ILogger<T> logger, Error error)
    {
        logger.LogError("{Error}", error.ToLogString());
    }
}