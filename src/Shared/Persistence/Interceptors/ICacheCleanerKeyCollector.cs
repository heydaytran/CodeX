namespace Persistence.Interceptors;

public interface ICacheCleanerKeyCollector
{
    public ErrorOr<IEnumerable<string>> KeysToClean(ChangeTracker changeTracker);
}