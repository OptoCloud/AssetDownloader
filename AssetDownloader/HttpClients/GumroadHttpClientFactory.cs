using System.Net;

namespace AssetDownloader.HttpClients;

public static class GumroadHttpClientFactory
{
    public static HttpClientHandler CreateHttpClientHandler()
    {
        string appSessionToken = Config.Instance.GumroadCookie ?? throw new InvalidOperationException("Gumroad cookie not set in config file");

        var authCookie = new Cookie("_gumroad_app_session", appSessionToken, "/", ".gumroad.com")
        {
            HttpOnly = true,
            Secure = true
        };

        var cookieContainer = new CookieContainer();

        cookieContainer.Add(authCookie);

        return new HttpClientHandler()
        {
            CookieContainer = cookieContainer,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
    }

    public static HttpClient CreateIndexHttpClient()
    {
        var client = new HttpClient(new HttpClientRateLimiterHandler(RateLimiters.GumroadIndexRateLimiter, CreateHttpClientHandler()))
        {
            Timeout = TimeSpan.FromMinutes(30)
        };

        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("User-Agent", "AssetDownloader/1.0");

        return client;
    }

    public static HttpClient CreateDownloadHttpClient()
    {
        var client = new HttpClient(new HttpClientRateLimiterHandler(RateLimiters.GumroadDownloadRateLimiter, CreateHttpClientHandler()))
        {
            Timeout = TimeSpan.FromMinutes(30)
        };

        client.DefaultRequestHeaders.Add("Accept", "*/*");
        client.DefaultRequestHeaders.Add("User-Agent", "AssetDownloader/1.0");

        return client;
    }
}
