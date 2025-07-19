using Endpoints.Extensions;

namespace Endpoints.CustomResults;

public class RangedContentResult(string content, string contentType) : IResult
{
    private readonly string _content = content ??
        throw new ArgumentNullException(nameof(content));

    private readonly string _contentType = contentType;

    /// <inherit />
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var fileResult = Results.File(
            Encoding.UTF8.GetBytes(_content),
            _contentType,
            enableRangeProcessing: httpContext.Request.IsTicketerRangeRequestEnabled());
        await fileResult.ExecuteAsync(httpContext);
    }
}