namespace AssetDownloader.Extensions;

internal static class StringExtensions
{
    public static string RemoveStart(this string str, string start)
    {
        if (str.StartsWith(start))
        {
            str = str[start.Length..];
        }

        return str;
    }
}
