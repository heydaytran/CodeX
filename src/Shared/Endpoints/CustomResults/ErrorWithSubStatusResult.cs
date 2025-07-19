namespace Endpoints.CustomResults;

public class ErrorWithSubStatusResult(int code, string? headerName = null) : IResult
{
    private const string DefaultHeaderName = "X-TKTR-SubStatus";

    private readonly int _code = code;
    private readonly string _headerName = headerName ?? DefaultHeaderName;

    /// <inheritdoc/>
    public Task ExecuteAsync(HttpContext httpContext)
    {
        httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        httpContext.Response.OnStarting(() =>
        {
            httpContext.Response.Headers.Append(_headerName, _code.ToString());

            return Task.CompletedTask;
        });

        return Task.CompletedTask;
    }
}
