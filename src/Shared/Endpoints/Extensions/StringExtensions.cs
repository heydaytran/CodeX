namespace Endpoints.Extensions;

public static class StringExtensions
{
    // Formats allowed by comms v1.
    private static readonly string[] AllowedDateTimeFormats =
    [
        "yyyyMMddHHmmss",
        "yyyyMMdd HHmmss",
        "yyyyMMdd HH-mm",
        "yyy-MM-dd",
        "dd/MM/yyyy",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-ddTHH:mm:ss.fff",
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy HH:mm",
        "yyyy-MM-dd HH:mm:ss",
        "yyyy-MM-dd",
        "yyyyMMddHHmmss.fff",
    ];

    public static Guid? AsGuid(this string? guidString)
    {
        if (string.IsNullOrEmpty(guidString) ||
            !Guid.TryParse(guidString, out Guid result))
        {
            return null;
        }

        return result;
    }

    public static DateTimeOffset? AsDateTimeOffset(this string? dateTimeString)
    {
        if (string.IsNullOrEmpty(dateTimeString))
        {
            return null;
        }

        if (DateTimeOffset.TryParseExact(dateTimeString, AllowedDateTimeFormats, null, DateTimeStyles.AssumeUniversal, out DateTimeOffset resultExact))
        {
            return resultExact;
        }

        if (DateTimeOffset.TryParse(dateTimeString, out DateTimeOffset resultGeneral))
        {
            return resultGeneral;
        }

        return null;
    }
}
