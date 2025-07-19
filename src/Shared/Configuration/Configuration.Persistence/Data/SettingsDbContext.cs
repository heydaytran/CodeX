using Configuration.Persistence.Models;
using Persistence.Extensions;

namespace Configuration.Persistence.Data;

public class SettingsDbContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;

    public SettingsDbContext(DbContextOptions<SettingsDbContext> options)
        : this(LoggerFactory.Create(c => { }), options)
    {
    }

    public SettingsDbContext(ILoggerFactory loggerFactory, DbContextOptions<SettingsDbContext> options)
        : base(options)
    {
        _loggerFactory = loggerFactory;
    }
    
    public DbSet<AppSetting> AppSettings => Set<AppSetting>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(_loggerFactory);
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
        optionsBuilder.EnableDetailedErrors();

        // Read only for now, so set 'no tracking' to benefit from the performance gain.
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.HasDefaultSchema("configurations");
        builder.ToSnakeCaseTables();
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(builder);
    }
}
