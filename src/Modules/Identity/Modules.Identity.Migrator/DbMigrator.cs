namespace Modules.Identity.Migrator;

public class MigrateDatabase
{
    public void Execute(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Identity");

        try
        {
            if (connectionString != null) new PostgresMigrator().Execute(connectionString);
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unexpected error has occurred");
        }
        finally
        {
            Log.CloseAndFlush();
        }
       
    }    
}