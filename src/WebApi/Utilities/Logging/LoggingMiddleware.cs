namespace WebApi.Utilities.Logging;

public class LoggingMiddleware(ILogger<LoggingMiddleware> logger) : IMiddleware
{
    private static readonly HashSet<string> SensitiveFields = new()
    {
        "password", "token", "cvv", "ssn", "creditCardNumber"
    };

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        context.Request.EnableBuffering(); // Allows request body to be read multiple times
        
        string requestBody = await ReadAndMaskBody(context.Request);
        logger.LogInformation("Incoming Request: {Method} {Path} {QueryString} Body: {RequestBody}",
            context.Request.Method, context.Request.Path, context.Request.QueryString, requestBody);

        var originalResponseBody = context.Response.Body;
        try
        {
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;
            
            await next(context);

            // Read, log, and reset response body
            string responseBody = await ReadAndMaskBody(context.Response);
            logger.LogInformation("Outgoing Response: {StatusCode} Body: {ResponseBody}",
                context.Response.StatusCode, responseBody);

            // Reset and copy back response body for the client
            memoryStream.Seek(0, SeekOrigin.Begin);
            await memoryStream.CopyToAsync(originalResponseBody);
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }
    }

    private async Task<string> ReadAndMaskBody(HttpRequest request)
    {
        if (request.ContentLength == null || request.ContentLength == 0)
            return string.Empty;

        request.EnableBuffering();
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        request.Body.Position = 0; // Reset for further reading

        return MaskSensitiveData(body);
    }

    private async Task<string> ReadAndMaskBody(HttpResponse response)
    {
        if (response.Body.Length == 0) return string.Empty;

        response.Body.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(response.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        response.Body.Seek(0, SeekOrigin.Begin);

        return MaskSensitiveData(body);
    }

    private string MaskSensitiveData(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return JsonSerializer.Serialize(MaskSensitiveFields(doc.RootElement));
        }
        catch
        {
            return "[Content Hidden]";
        }
    }

    private JsonElement MaskSensitiveFields(JsonElement element)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            var dict = new Dictionary<string, JsonElement>();

            foreach (var property in element.EnumerateObject())
            {
                if (SensitiveFields.Contains(property.Name.ToLower()))
                {
                    dict[property.Name] = JsonSerializer.SerializeToElement("***REDACTED***");
                }
                else
                {
                    dict[property.Name] = MaskSensitiveFields(property.Value);
                }
            }
            return JsonSerializer.SerializeToElement(dict);
        }
        return element;
    }
}
