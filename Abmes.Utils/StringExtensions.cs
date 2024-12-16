namespace Abmes.Utils;

public static class StringExtensions
{
    public static string Truncate(this string value, int maxLength)
    {
        return
            string.IsNullOrEmpty(value) || value.Length <= maxLength
            ? value
            : value[..maxLength];
    }
}