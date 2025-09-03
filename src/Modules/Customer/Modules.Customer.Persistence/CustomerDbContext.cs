using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.Extensions;
using Modules.Customer.Domain.Entities;
using Modules.Customer.Persistence.Entities;

namespace Modules.Customer.Persistence;

public class CustomerDbContext: DbContext
{
    private readonly ILoggerFactory _loggerFactory;

    public CustomerDbContext(ILoggerFactory loggerFactory, DbContextOptions<CustomerDbContext> options)
        : base(options)
    {
        _loggerFactory = loggerFactory;
    }

    public DbSet<ContactInformation> ContactInformations => Set<ContactInformation>();
    public DbSet<Domain.Entities.Customer> Customers => Set<Domain.Entities.Customer>();
    public DbSet<CustomerEvent> CustomerEvents => Set<CustomerEvent>();
    
    /// <inheritdoc />
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
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

        // Read only for now, so set 'no tracking' to benefit from the performance gain.
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }
}