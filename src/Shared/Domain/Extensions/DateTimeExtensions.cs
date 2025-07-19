using System.Globalization;

namespace Domain.Extensions;

public static class DateTimeExtensions
{
    private const string ZuluDateTimeStringFormat = "yyyy-MM-ddTHH:mm:ss.fff";
    public static DateTime FromUtcTimeString(string zuluDateTimeString)
    {
        var utcDateTime = DateTime.ParseExact(zuluDateTimeString, ZuluDateTimeStringFormat,
            CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

        return utcDateTime;
    }
    
    public static DateTime UnixEpochToDateTime(long value)
    {
        return DateTime.UnixEpoch.AddSeconds(value);
    }
}