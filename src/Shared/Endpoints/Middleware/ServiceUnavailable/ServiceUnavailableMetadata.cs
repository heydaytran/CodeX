namespace Endpoints.Middleware.ServiceUnavailable;

public class ServiceUnavailableMetadata
{
    public required Func<HttpContext, bool> Condition { get; set; }

    public Func<HttpResponse, Task>? WriteResponseAsync { get; set; }
}