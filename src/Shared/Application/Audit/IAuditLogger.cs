namespace Application.Audit;

public interface IAuditLogger
{
    Task LogAsync(string action, string username, string result, string? reason = null);
}