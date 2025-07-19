namespace Domain.Extensions;

public static class StringExtensions
{
    public static string TrimStart(this string target, string trimChars, StringComparison comparison)
    {
        if (!string.IsNullOrEmpty(target) && target.StartsWith(trimChars, comparison))
        {
            target = target[trimChars.Length..];
        }

        return target;
    }

    public static string TrimEnd(this string target, string trimChars, StringComparison comparison)
    {
        if (!string.IsNullOrEmpty(target) && target.EndsWith(trimChars, comparison))
        {
            target = target[..(target.Length - trimChars.Length)];
        }

        return target;
    }

    public static string TrimStart(this string target, char charToTrim, int maxCharactersToRemove)
    {
        if (string.IsNullOrEmpty(target))
        {
            return target;
        }

        // Loop through the string until we find a character that is not charToStrip
        // or we reach the max characters to remove.
        int startToReturn = 0;
        while (startToReturn < target.Length && target[startToReturn] == charToTrim && maxCharactersToRemove > 0)
        {
            startToReturn++;
            maxCharactersToRemove--;
        }

        return target[startToReturn..];
    }

    public static string TrimEnd(this string target, char charToTrim, int maxCharactersToRemove)
    {
        if (string.IsNullOrEmpty(target))
        {
            return target;
        }

        // Loop through the string from the end towards the beginning.
        int endToReturn = target.Length - 1;
        while (endToReturn >= 0 && target[endToReturn] == charToTrim && maxCharactersToRemove > 0)
        {
            endToReturn--;
            maxCharactersToRemove--;
        }

        return target[..(endToReturn + 1)];
    }

    public static string Md5(this string value, int maxLength = int.MaxValue)
    {
        if (string.IsNullOrEmpty(value))
        {
            return string.Empty;
        }

        var len = value.Length;
        var str = len > maxLength ? value[(len - maxLength)..] : value;

        byte[] bytes = MD5.HashData(Encoding.UTF8.GetBytes(str));

        char[] hexChars = new char[bytes.Length * 2];

        for (int i = 0; i < bytes.Length; i++)
        {
            byte b = bytes[i];
            hexChars[i * 2] = GetHexChar(b >> 4);  // High nibble
            hexChars[i * 2 + 1] = GetHexChar(b & 0xF);  // Low nibble
        }

        return new string(hexChars);
    }

    // Helper method to get the hexadecimal character for a byte value
    private static char GetHexChar(int nibble) => 
        (char)(nibble < 10 ? nibble + '0' : nibble - 10 + 'a');
}
