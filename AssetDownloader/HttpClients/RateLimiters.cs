using System.Threading.RateLimiting;

namespace AssetDownloader.HttpClients;

public static class RateLimiters
{
    private static TokenBucketRateLimiter CreateRateLimiter(int tokenLimit, int queueLimit, int tokensPerPeriod, TimeSpan replenishmentPeriod)
    {
        var options = new TokenBucketRateLimiterOptions()
        {
            TokenLimit = tokenLimit,
            QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
            QueueLimit = queueLimit,
            ReplenishmentPeriod = replenishmentPeriod,
            TokensPerPeriod = tokensPerPeriod,
            AutoReplenishment = true
        };

        return new TokenBucketRateLimiter(options);
    }

    public static TokenBucketRateLimiter BoothIndexRateLimiter { get; } = CreateRateLimiter(1, 1, 2, TimeSpan.FromSeconds(1));
    public static TokenBucketRateLimiter BoothDownloadRateLimiter { get; } = CreateRateLimiter(1, 1, 2, TimeSpan.FromSeconds(1));
    public static TokenBucketRateLimiter GumroadIndexRateLimiter { get; } = CreateRateLimiter(1, 1, 2, TimeSpan.FromSeconds(1));
    public static TokenBucketRateLimiter GumroadDownloadRateLimiter { get; } = CreateRateLimiter(1, 1, 2, TimeSpan.FromSeconds(1));
}
