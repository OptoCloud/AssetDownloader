using System.Globalization;
using System.Net;
using System.Threading.RateLimiting;

namespace AssetDownloader.HttpClients;

public sealed class HttpClientRateLimiterHandler(RateLimiter limiter, HttpClientHandler httpClientHandler) : DelegatingHandler(httpClientHandler)
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using RateLimitLease lease = await limiter.AcquireAsync(permitCount: 1, cancellationToken);

        if (lease.IsAcquired)
        {
            return await base.SendAsync(request, cancellationToken);
        }

        var response = new HttpResponseMessage(HttpStatusCode.TooManyRequests);
        if (lease.TryGetMetadata(MetadataName.RetryAfter, out TimeSpan retryAfter))
        {
            response.Headers.Add("Retry-After", ((int)retryAfter.TotalSeconds).ToString(NumberFormatInfo.InvariantInfo));
        }

        return response;
    }
}