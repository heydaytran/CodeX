namespace Persistence.Options;

public class CachedRepositoryOptions<TRepository>
{
    public bool Enabled { get; set; } = true;

    public TimeSpan? AbsoluteExpirationRelativeToNow { get; set; } = TimeSpan.FromMinutes(3);
}