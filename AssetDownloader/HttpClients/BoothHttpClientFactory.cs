using System.Net;
using System.Threading.RateLimiting;

namespace AssetDownloader.HttpClients;

public static class BoothHttpClientFactory
{
    private static HttpClientHandler CreateHttpClientHandler()
    {
        string plazaSessionToken = Config.Instance.BoothCookie ?? throw new InvalidOperationException("Booth cookie not set in config file");

        var authCookie = new Cookie("_plaza_session_nktz7u", plazaSessionToken, "/", ".booth.pm")
        {
            HttpOnly = true,
            Secure = true
        };
        var showAdultContentCookie = new Cookie("adult", "t", "/", ".booth.pm")
        {
            HttpOnly = true,
            Secure = true
        };

        var cookieContainer = new CookieContainer();

        cookieContainer.Add(authCookie);
        cookieContainer.Add(showAdultContentCookie);

        return new HttpClientHandler()
        {
            CookieContainer = cookieContainer,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
    }

    public static HttpClient CreateIndexHttpClient()
    {
        var client = new HttpClient(new HttpClientRateLimiterHandler(RateLimiters.BoothIndexRateLimiter, CreateHttpClientHandler()))
        {
            Timeout = TimeSpan.FromMinutes(30)
        };

        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("User-Agent", "AssetDownloader/1.0");

        return client;
    }

    public static HttpClient CreateDownloadHttpClient()
    {
        var client = new HttpClient(new HttpClientRateLimiterHandler(RateLimiters.BoothDownloadRateLimiter, CreateHttpClientHandler()))
        {
            Timeout = TimeSpan.FromMinutes(30)
        };

        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("User-Agent", "AssetDownloader/1.0");

        return client;
    }
}
