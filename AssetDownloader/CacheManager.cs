using System.Text.Json;

namespace AssetDownloader;

public static class CacheManager
{
    private static string GetCachePath(Uri requestUri)
    {
        string cacheDir = Path.Combine("cache", requestUri.Host);
        string cacheFile = Path.Combine(cacheDir, requestUri.AbsolutePath.TrimStart('/'));

        if (!Directory.Exists(cacheDir))
        {
            Directory.CreateDirectory(cacheDir);
        }

        return cacheFile;
    }

    public static async Task CacheResponseText(Uri requestUri, string responseText)
    {
        string cachePath = GetCachePath(requestUri);

        await File.WriteAllTextAsync(cachePath, responseText);
    }

    public static async Task<string?> GetCachedResponseText(Uri requestUri)
    {
        string cachePath = GetCachePath(requestUri);

        if (!File.Exists(cachePath))
        {
            return null;
        }

        return await File.ReadAllTextAsync(cachePath);
    }

    public static async Task CacheResponseJson(Uri requestUri, object responseJson)
    {
        string cachePath = GetCachePath(requestUri);

        await File.WriteAllTextAsync(cachePath, JsonSerializer.Serialize(responseJson));
    }

    public static async Task<T?> GetCachedResponseJson<T>(Uri requestUri)
    {
        string cachePath = GetCachePath(requestUri);

        if (!File.Exists(cachePath))
        {
            return default;
        }

        string json = await File.ReadAllTextAsync(cachePath);

        return JsonSerializer.Deserialize<T>(json);
    }

    public static async Task CacheResponseStream(Uri requestUri, Stream responseStream)
    {
        string cachePath = GetCachePath(requestUri);

        using var fileStream = File.Create(cachePath);
        await responseStream.CopyToAsync(fileStream);
    }

    public static Stream? GetCachedResponseStream(Uri requestUri)
    {
        string cachePath = GetCachePath(requestUri);

        if (!File.Exists(cachePath))
        {
            return null;
        }

        return File.OpenRead(cachePath);
    }
}
