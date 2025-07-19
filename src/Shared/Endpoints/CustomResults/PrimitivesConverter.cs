using System.Buffers.Text;
using System.Runtime.InteropServices;

namespace Endpoints.CustomResults;

public static class PrimitivesConverter
{
    private const string DateTimeFormatCompactFullPrecision = "yyyyMMddHHmmss.fff";
    private const string DateTimeFormatCompact = "yyyyMMddHHmmss";

    private const char UnderscoreChar = '_';
    private const char HyphenChar = '-';
    private const char EqualsChar = '=';
    private const char PlusChar = '+';
    private const char ForwardSlashChar = '/';
    private const byte ForwardSlashByte = (byte)'/';
    private const byte PlusByte = (byte)'+';

    public static string Convert(Guid guid)
    {
        Span<byte> idBytes = stackalloc byte[16];
        Span<byte> base64Bytes = stackalloc byte[24];

        MemoryMarshal.TryWrite(idBytes, in guid);
        Base64.EncodeToUtf8(idBytes, base64Bytes, out _, out _);

        Span<char> result = stackalloc char[22];
        for (var i = 0; i < 22; i++)
        {
            result[i] = base64Bytes[i] switch
            {
                ForwardSlashByte => UnderscoreChar,
                PlusByte => HyphenChar,
                _ => (char)base64Bytes[i],
            };
        }

        return new string(result);
    }

    public static string Convert(DateTimeOffset dateTimeOffset, bool fullPrecision = true)
    {
        return fullPrecision ?
            dateTimeOffset.ToString(DateTimeFormatCompactFullPrecision) :
            dateTimeOffset.ToString(DateTimeFormatCompact);
    }

    public static string Convert(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        return new string(value.Where(XmlConvert.IsXmlChar).ToArray());
    }

    public static bool TryParse(ReadOnlySpan<char> guidString, out Guid guid)
    {
        if (Guid.TryParse(guidString, out guid))
        {
            return true;
        }

        if (guidString.Length != 22)
        {
            return false;
        }

        Span<char> base64Chars = stackalloc char[24];

        for (var i = 0; i < 22; i++)
        {
            base64Chars[i] = guidString[i] switch
            {
                UnderscoreChar => ForwardSlashChar,
                HyphenChar => PlusChar,
                _ => guidString[i],
            };
        }

        base64Chars[22] = EqualsChar;
        base64Chars[23] = EqualsChar;

        Span<byte> idBytes = stackalloc byte[16];
        if (!System.Convert.TryFromBase64Chars(base64Chars, idBytes, out _))
        {
            return false;
        }

        guid = new Guid(idBytes);

        return true;
    }

    public static bool TryParse(string? intString, out int? number, bool required = true)
    {
        number = null;

        if (string.IsNullOrEmpty(intString))
        {
            return !required;
        }

        if (!int.TryParse(intString, out int numberNotNullable))
        {
            return false;
        }

        number = numberNotNullable;

        return true;
    }
}