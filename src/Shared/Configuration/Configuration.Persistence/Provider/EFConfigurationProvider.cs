using Configuration.Persistence.Models;

namespace Configuration.Persistence.Provider;

internal sealed class EfConfigurationProvider<TDbContext>(EfConfigurationSource<TDbContext> configurationSource) : ConfigurationProvider, IDisposable
    where TDbContext : DbContext
{
    private readonly EfConfigurationSource<TDbContext> _configurationSource = configurationSource ?? 
        throw new ArgumentNullException(nameof(configurationSource));

    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private byte[] _lastComputedHash = new byte[20];
    private Task? _watchDbTask;
    private bool _disposed;

    public override void Load()
    {
        if (_watchDbTask != null)
        {
            return;
        }

        try
        {
            Data = GetDataAsync().Result;
            _lastComputedHash = EfConfigurationProvider<TDbContext>.ComputeHash(Data);
        }
        catch (Exception ex)
        {
            var exceptionContext = new EfConfigurationLoadExceptionContext<TDbContext>(_configurationSource, ex);
            _configurationSource.OnLoadException?.Invoke(exceptionContext);
            if (!exceptionContext.Ignore)
            {
                throw;
            }
        }

        var cancellationToken = _cancellationTokenSource.Token;
        if (_configurationSource.ReloadOnChange)
        {
            _watchDbTask = Task.Run(() => WatchDatabaseAsync(cancellationToken), cancellationToken);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
        _disposed = true;

        GC.SuppressFinalize(this);
    }

    private async Task WatchDatabaseAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_configurationSource.PollingInterval, cancellationToken);
                await ReloadData();
            }
            catch (Exception ex)
            {
                var exceptionContext = new EfConfigurationLoadExceptionContext<TDbContext>(_configurationSource, ex);
                _configurationSource.OnLoadException?.Invoke(exceptionContext);
                if (!exceptionContext.Ignore)
                {
                    throw;
                }
            }
        }
    }

    private async Task ReloadData()
    {
        IDictionary<string, string?> actualData = await GetDataAsync();

        byte[] computedHash = EfConfigurationProvider<TDbContext>.ComputeHash(actualData);
        if (!computedHash.SequenceEqual(_lastComputedHash))
        {
            Data = actualData;
            OnReload();
        }
        _lastComputedHash = computedHash;
    }

    private async Task<IDictionary<string, string?>> GetDataAsync(CancellationToken cancellationToken = default)
    {
        using TDbContext? dbContext = CreateDbContext();
        dbContext.Database.EnsureCreated();

        IQueryable<AppSetting> settings = GetSettings(dbContext);

        var data = settings.Any() ? await settings.ToDictionaryAsync(c => c.Name, c => c.Value, cancellationToken) : [];

        return
            _configurationSource.ValueFormatter != null ? 
            _configurationSource.ValueFormatter.FormatValues(data) : data;
    }

    private static byte[] ComputeHash(IDictionary<string, string?> dict)
    {
        List<byte> byteDict = [];
        foreach (KeyValuePair<string, string?> item in dict)
        {
            byteDict.AddRange(Encoding.Unicode.GetBytes($"{item.Key}{item.Value}"));
        }

        return System.Security.Cryptography.SHA1.HashData([.. byteDict]);
    }

    private IQueryable<AppSetting> GetSettings(TDbContext dbContext)
    {
        DbSet<AppSetting> settings = dbContext.Set<AppSetting>();

        var query = settings
            .Where(s => 
                (s.App == _configurationSource.Application || s.App == "Global") && 
                (s.Target == null || s.Target == _configurationSource.Target) &&
                !s.IsDeleted);
        
        // Target specific app config takes precedence.
        return query
            .GroupBy(s => new { s.Name, s.Target })
            .Select(s => s.FirstOrDefault(s => s.Target == _configurationSource.Target) ?? s.First());
    }

    private TDbContext CreateDbContext()
    {
        DbContextOptionsBuilder<TDbContext> builder = new();
        _configurationSource.OptionsAction(builder);

        var dbContext = (TDbContext?)Activator.CreateInstance(typeof(TDbContext), [builder.Options]);

        return dbContext ?? throw new InvalidOperationException($"Failed to create db context");
    }
}
