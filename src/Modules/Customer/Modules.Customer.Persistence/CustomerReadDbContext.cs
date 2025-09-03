using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Extensions;
using Modules.Customer.Persistence.ReadModels;

namespace Modules.Customer.Persistence;

public class CustomerReadDbContext : DbContext
{
    private readonly ILoggerFactory _loggerFactory;

    public CustomerReadDbContext(ILoggerFactory loggerFactory, DbContextOptions<CustomerReadDbContext> options)
        : base(options)
    {
        _loggerFactory = loggerFactory;
    }

    public DbSet<CustomerProfileReadModel> CustomerProfiles => Set<CustomerProfileReadModel>();
    public DbSet<CustomerChangeReadModel> CustomerChanges => Set<CustomerChangeReadModel>();
    public DbSet<CustomerNotificationPreferencesReadModel> CustomerNotificationPreferences => Set<CustomerNotificationPreferencesReadModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("customer");
        modelBuilder.ToSnakeCaseTables();
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseLoggerFactory(_loggerFactory);
#if DEBUG
        optionsBuilder.EnableSensitiveDataLogging();
#endif
        optionsBuilder.EnableDetailedErrors();
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}
