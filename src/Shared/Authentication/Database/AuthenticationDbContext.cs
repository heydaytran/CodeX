namespace Authentication.Database;

public class AuthenticationDbContext : IdentityDbContext<User>, IDataProtectionKeyContext
{
    private readonly ILoggerFactory _loggerFactory;

    public AuthenticationDbContext(ILoggerFactory loggerFactory, DbContextOptions<AuthenticationDbContext> options)
        : base(options)
    {
        _loggerFactory = loggerFactory;
    }

    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();
    
    /// <inheritdoc />
    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.HasDefaultSchema("identity");
        
        modelBuilder.ToSnakeCaseTables();
        
        modelBuilder.ApplyConfigurationsFromAssembly(AssemblyReference.Assembly);
        
        modelBuilder.Entity<DataProtectionKey>(entity =>
        {
            entity.ToTable("data_protection_keys");
            entity.HasKey(k => k.Id);
        });
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