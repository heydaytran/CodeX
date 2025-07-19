namespace Modules.Customer.Migrator;

public class MigrateDatabase
{
    public void Execute(IConfiguration configuration)
    {
        string? connectionString = configuration.GetConnectionString("Default");

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