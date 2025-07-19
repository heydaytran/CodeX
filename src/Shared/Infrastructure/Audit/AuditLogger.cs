using Application.Audit;

namespace Infrastructure.Audit;

public class AuditLogger : IAuditLogger
{
    private readonly ILogger<AuditLogger> _logger;

    public AuditLogger(ILogger<AuditLogger> logger)
    {
        _logger = logger;
    }

    public Task LogAsync(string action, string username, string result, string? reason = null)
    {
        _logger.LogInformation("AUDIT | Action: {Action} | User: {User} | Result: {Result} | Reason: {Reason}",
            action, username, result, reason ?? "N/A");

        return Task.CompletedTask;
    }
}
