namespace Application;

public static class SessionIdGenerator
{
    private static readonly char[] Base62Chars = 
        "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

    public static string Generate()
    {
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(); // Get current timestamp
        var random = new Random();
        var sb = new StringBuilder();

        // Convert timestamp to Base62
        while (timestamp > 0)
        {
            sb.Insert(0, Base62Chars[timestamp % 62]);
            timestamp /= 62;
        }

        // Append random 5 Base62 characters for uniqueness
        for (int i = 0; i < 5; i++)
        {
            sb.Append(Base62Chars[random.Next(Base62Chars.Length)]);
        }

        return sb.ToString();
    }
}
