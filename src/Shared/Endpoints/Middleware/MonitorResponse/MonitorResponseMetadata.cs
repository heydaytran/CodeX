namespace Endpoints.Middleware.MonitorResponse;

public record MonitorResponseMetadata
{
    public static MonitorResponseMetadata Instance { get; } = new();
}