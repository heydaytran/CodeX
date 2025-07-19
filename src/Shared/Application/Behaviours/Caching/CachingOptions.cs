namespace Application.Behaviours.Caching;

public class CachingOptions
{
    public bool Enabled { get; set; } = true;

    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromHours(1);

    public TimeSpan? AbsoluteExpiration { get; set; }
}