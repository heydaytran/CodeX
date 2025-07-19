namespace Configuration.Persistence.Models;

public class AppSetting
{
    public required Guid Id { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime LastActivityAt { get; set; }

    public required string App { get; set; }

    public required string Name { get; set; }

    public string? Value { get; set; }

    public string? Note { get; set; }

    public string? Target { get; set; }

    public bool IsDeleted { get; set; }
}
