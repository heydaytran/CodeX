namespace Endpoints.Middleware.ServiceUnavailable;

public record ServiceUnavailableMiddlewareOptions
{
    public bool Enabled { get; set; }
}