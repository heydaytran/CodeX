namespace Endpoints.Middleware.MonitorResponse;

public class MonitorResponseMiddlewareOptions
{
    public string HeaderName { get; set; } = "Monitor";

    public string ResponseContent { get; set; } = "#ALLOK#";
}