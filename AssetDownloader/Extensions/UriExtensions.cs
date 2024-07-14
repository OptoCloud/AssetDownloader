using System.ComponentModel;
using System.Web;

namespace AssetDownloader.Extensions;

internal static class UriExtensions
{
    public static string? GetQueryValue(this Uri uri, string key)
    {
        var query = HttpUtility.ParseQueryString(uri.Query);
        return query[key];
    }

    // Generic method to get the value of a query parameter as integer type (int, uint, long, ulong)
    public static T? GetQueryValue<T>(this Uri uri, string key) where T : struct
    {
        string? value = uri.GetQueryValue(key);
        if (value is null)
        {
            return null;
        }

        if (TypeDescriptor.GetConverter(typeof(T)).ConvertFromString(value) is not T result)
        {
            return null;
        }

        return result;
    }
}
