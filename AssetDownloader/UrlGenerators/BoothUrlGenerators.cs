using AssetDownloader.Enums;

namespace AssetDownloader.UrlGenerators;

internal static class BoothUrlGenerators
{
    private static string GetBoothRegionString(BoothRegion region)
    {
        return region switch
        {
            BoothRegion.English => "en",
            BoothRegion.Japanese => "ja",
            BoothRegion.Korean => "ko",
            BoothRegion.SimplifiedChinese => "zh-cn",
            BoothRegion.TraditionalChinese => "zh-tw",
            _ => throw new NotImplementedException(),
        };
    }

    public static Uri GetAccountLibraryUrl(uint page = 1) => new($"https://accounts.booth.pm/library?page={page}");
    public static Uri GetAccountOrdersUrl(uint page = 1) => new($"https://accounts.booth.pm/orders?page={page}");
    public static Uri GetAccountOrderUrl(uint orderId) => new($"https://accounts.booth.pm/orders/{orderId}");
    public static Uri GetStoreUrl(string creatorId, uint page = 1) => new($"https://{creatorId}.booth.pm/items?page={page}");
    public static Uri GetItemUrl(uint itemId, BoothRegion region = BoothRegion.English) => new($"https://booth.pm/{GetBoothRegionString(region)}/items/{itemId}");
    public static Uri GetItemUrl(string creatorId, uint itemId) => new($"https://{creatorId}.booth.pm/items/{itemId}");
    public static Uri GetItemJsonUrl(uint itemId, BoothRegion region = BoothRegion.English) => new(GetItemUrl(itemId, region) + ".json");
    public static Uri GetItemJsonUrl(string creatorId, uint itemId) => new(GetItemUrl(creatorId, itemId) + ".json");
    public static Uri GetUserImageUrl(uint creatorS2Id, Guid imageId, string ext = "jpg") => new($"https://s2.booth.pm/users/{creatorS2Id}/icon_image/{imageId}_base_resized.{ext}");
    public static Uri GetItemImageUrl(Guid shopS2BucketId, uint itemId, Guid itemS2EntryId, string ext = "jpg") => new($"https://s2.booth.pm/{shopS2BucketId}/i/{itemId}/{itemS2EntryId}_base_resized.{ext}");
    public static Uri GetDownloadableUrl(uint downloadableId) => new($"https://booth.pm/downloadables/{downloadableId}");
}
