using Npgsql;

namespace Persistence.Options;

public sealed class ConnectionStringOptions
{
    /// <summary>
    /// Gets the connection string value.
    /// </summary>
    public string Value { get; internal set; } = string.Empty;

    public static implicit operator string(ConnectionStringOptions connectionString) => connectionString.Value;

    /// <inheritdoc/>
    public override string ToString()
    {
        try
        {
            var builder = new NpgsqlConnectionStringBuilder(Value);
            builder.Remove("Password"); // Removes the password for security purposes
            return builder.ToString();
        }
        catch (Exception)
        {
            return "Unavailable";
        }
    }
}