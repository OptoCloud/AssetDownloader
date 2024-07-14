using AssetDownloader.Enums;
using System.Text.RegularExpressions;
using System.Web;

namespace AssetDownloader.UrlParsers;

internal sealed partial class BoothUrlParsers
{
    private static bool TryGetPage(Uri url, string queryKey, out uint page)
    {
        page = 1;

        if (string.IsNullOrEmpty(url.Query)) return true; // No query string

        var nvc = HttpUtility.ParseQueryString(url.Query);
        if (nvc is null)
        {
            Console.WriteLine($"Failed to parse query string: {url.Query}");
            return false;
        }

        var pageValue = nvc.Get(queryKey);
        if (pageValue is null) return true; // No page value

        if (!uint.TryParse(pageValue, out page))
        {
            Console.WriteLine($"Invalid page value: {pageValue}");
            return false;
        }

        return true;
    }

    public static bool TryValidateAndParseLibraryUrl(Uri url, out uint page)
    {
        page = 1;

        if (url.Host is not "accounts.booth.pm") return false;

        if (!TryGetPage(url, "page", out page)) return false;

        if (url.Segments is not ["/", "library" or "library/"]) return false;

        return true;
    }

    public static bool TryValidateAndParseOrdersUrl(Uri url, out uint page)
    {
        page = 1;

        if (url.Host is not "accounts.booth.pm") return false;

        if (!TryGetPage(url, "page", out page)) return false;

        if (url.Segments is not ["/", "orders" or "orders/"]) return false;

        return true;
    }

    public static bool TryValidateAndParseOrderUrl(Uri url, out uint orderId)
    {
        orderId = 0;

        if (url.Host is not "accounts.booth.pm") return false;

        if (url.Segments is not ["/", "orders/", string orderIdStr]) return false;

        if (orderIdStr[^1] is '/') orderIdStr = orderIdStr[..^1]; // Remove trailing '/' if present

        if (!uint.TryParse(orderIdStr, out orderId)) return false;

        return true;
    }

    public static bool TryValidateAndParseStoreUrl(Uri url, out string storeId, out uint page)
    {
        page = 1;
        storeId = "";

        if (!url.Host.EndsWith(".booth.pm")) return false;

        if (url.Segments is not ["/"] or ["/", "items" or "items/"]) return false;

        if (!TryGetPage(url, "page", out page)) return false;

        storeId = url.Host[..^9];

        return storeId.Length > 0;
    }

    public static bool TryValidateAndParseItemUrl(Uri url, out uint itemId, out BoothRegion region)
    {
        itemId = 0;
        region = BoothRegion.English;

        if (url.Host is not "booth.pm") return false;

        if (url.Segments is not ["/", string regionStr, "items/", string itemIdStr]) return false;

        switch (regionStr)
        {
            case "en/":
                region = BoothRegion.English;
                break;
            case "ja/":
                region = BoothRegion.Japanese;
                break;
            case "ko/":
                region = BoothRegion.Korean;
                break;
            case "zh-cn/":
                region = BoothRegion.SimplifiedChinese;
                break;
            case "zh-tw/":
                region = BoothRegion.TraditionalChinese;
                break;
            default:
                return false;
        }

        if (itemIdStr[^1] is '/') itemIdStr = itemIdStr[..^1]; // Remove trailing '/' if present

        if (!uint.TryParse(itemIdStr, out itemId)) return false;

        return true;
    }

    public static bool TryValidateAndParseItemUrl(Uri url, out string storeId, out uint itemId)
    {
        itemId = 0;
        storeId = "";

        if (!url.Host.EndsWith(".booth.pm")) return false;

        if (url.Segments is not ["/", "items/", string itemIdStr]) return false;

        if (itemIdStr[^1] is '/') itemIdStr = itemIdStr[..^1]; // Remove trailing '/' if present

        if (!uint.TryParse(itemIdStr, out itemId)) return false;

        storeId = url.Host[..^9];

        return storeId.Length > 0;
    }

    public static bool TryValidateAndParseItemUrl(Uri url, out uint itemId)
    {
        if (TryValidateAndParseItemUrl(url, out itemId, out _)) return true;
        if (TryValidateAndParseItemUrl(url, out _, out itemId)) return true;
        return false;
    }

    public static bool TryValidateAndParseItemJsonUrl(Uri url, out uint itemId, out BoothRegion region)
    {
        itemId = 0;
        region = BoothRegion.English;

        if (!url.Segments[^1].EndsWith(".json", StringComparison.OrdinalIgnoreCase)) return false;

        url = new Uri(url.ToString()[..^5]); // FIXME: This is a hack, but it works

        return TryValidateAndParseItemUrl(url, out itemId, out region);
    }

    public static bool TryValidateAndParseItemJsonUrl(Uri url, out string storeId, out uint itemId)
    {
        itemId = 0;
        storeId = "";

        if (!url.Segments[^1].EndsWith(".json", StringComparison.OrdinalIgnoreCase)) return false;

        url = new Uri(url.ToString()[..^5]); // FIXME: This is a hack, but it works

        return TryValidateAndParseItemUrl(url, out storeId, out itemId);
    }

    public static bool TryValidateAndParseItemJsonUrl(Uri url, out uint itemId)
    {
        if (TryValidateAndParseItemJsonUrl(url, out itemId, out _)) return true;
        if (TryValidateAndParseItemJsonUrl(url, out _, out itemId)) return true;
        return false;
    }

    [GeneratedRegex(@"^(\d+)x(\d+)(?:_a(\d+))?(?:_g(\d+))?/$")]
    private static partial Regex GetPximgSizeRegex();

    private static bool TryValidateImageName(string fullName, out Guid guid, out string ext)
    {
        if (fullName.Split('.') is not [string beforeDot, string afterDot])
        {
            guid = Guid.Empty;
            ext = "";
            return false;
        }

        ext = afterDot;

        if (beforeDot.Split('_') is not [string imageId, "base", "resized"])
        {
            guid = Guid.Empty;
            return false;
        }

        return Guid.TryParse(imageId, out guid);
    }

    public static bool TryValidateAndParseS2UserImageUrl(Uri url, out uint creatorS2Id, out Guid imageId, out string ext)
    {
        if (url.Host is not "s2.booth.pm")
        {
            creatorS2Id = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        if (url.Segments is not ["/", "users/", string creatorS2IdStr, "icon_image/", string imageFileName])
        {
            creatorS2Id = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        if (!uint.TryParse(creatorS2IdStr[..^1], out creatorS2Id))
        {
            creatorS2Id = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        return TryValidateImageName(imageFileName, out imageId, out ext);
    }

    public static bool TryValidateAndParseS2ItemImageUrl(Uri url, out Guid itemBucketId, out uint itemId, out Guid imageId, out string ext)
    {
        if (url.Host is not "s2.booth.pm")
        {
            itemBucketId = Guid.Empty;
            itemId = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        if (url.Segments is not ["/", string itemBucketIdStr, "i/", string itemIdStr, string imageFileName])
        {
            itemBucketId = Guid.Empty;
            itemId = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        if (!Guid.TryParse(itemBucketIdStr[..^1], out itemBucketId))
        {
            itemId = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        if (!uint.TryParse(itemIdStr[..^1], out itemId))
        {
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        return TryValidateImageName(imageFileName, out imageId, out ext);
    }

    public static bool TryValidateAndParsePximgUserImageUrl(Uri url, out uint creatorS2Id, out Guid imageId, out string ext)
    {
        if (url.Host is not "booth.pximg.net")
        {
            creatorS2Id = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        if (url.Segments is not ["/", "users/", string creatorS2IdStr, "icon_image/", string imageFileName])
        {
            if (url.Segments is not ["/", "c/", string dimensions, "users/", string creatorS2IdStr2, "icon_image/", string imageFileName2])
            {
                creatorS2Id = 0;
                imageId = Guid.Empty;
                ext = "";
                return false;
            }

            if (GetPximgSizeRegex().Match(dimensions) is not { Success: true })
            {
                creatorS2Id = 0;
                imageId = Guid.Empty;
                ext = "";
                return false;
            }

            creatorS2IdStr = creatorS2IdStr2;
            imageFileName = imageFileName2;
        }

        if (!uint.TryParse(creatorS2IdStr[..^1], out creatorS2Id))
        {
            creatorS2Id = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        return TryValidateImageName(imageFileName, out imageId, out ext);
    }

    public static bool TryValidateAndParsePximgItemImageUrl(Uri url, out Guid itemBucketId, out uint itemId, out Guid imageId, out string ext)
    {
        if (url.Host is not "booth.pximg.net")
        {
            itemBucketId = Guid.Empty;
            itemId = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        if (url.Segments is not ["/", string itemBucketIdStr, "i/", string itemIdStr, string imageFileName])
        {
            if (url.Segments is not ["/", "c/", string dimensions, string itemBucketIdStr2, "i/", string itemIdStr2, string imageFileName2])
            {
                itemBucketId = Guid.Empty;
                itemId = 0;
                imageId = Guid.Empty;
                ext = "";
                return false;
            }

            if (GetPximgSizeRegex().Match(dimensions) is not { Success: true })
            {
                itemBucketId = Guid.Empty;
                itemId = 0;
                imageId = Guid.Empty;
                ext = "";
                return false;
            }

            itemBucketIdStr = itemBucketIdStr2;
            itemIdStr = itemIdStr2;
            imageFileName = imageFileName2;
        }

        if (!Guid.TryParse(itemBucketIdStr[..^1], out itemBucketId))
        {
            itemId = 0;
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        if (!uint.TryParse(itemIdStr[..^1], out itemId))
        {
            imageId = Guid.Empty;
            ext = "";
            return false;
        }

        return TryValidateImageName(imageFileName, out imageId, out ext);
    }

    public static bool TryValidateAndParseUserImageUrl(Uri url, out uint creatorS2Id, out Guid imageId, out string ext)
    {
        if (TryValidateAndParseS2UserImageUrl(url, out creatorS2Id, out imageId, out ext)) return true;
        if (TryValidateAndParsePximgUserImageUrl(url, out creatorS2Id, out imageId, out ext)) return true;
        return false;
    }

    public static bool TryValidateAndParseItemImageUrl(Uri url, out Guid itemBucketId, out uint itemId, out Guid imageId, out string ext)
    {
        if (TryValidateAndParseS2ItemImageUrl(url, out itemBucketId, out itemId, out imageId, out ext)) return true;
        if (TryValidateAndParsePximgItemImageUrl(url, out itemBucketId, out itemId, out imageId, out ext)) return true;
        return false;
    }

    public static bool TryValidateAndParseDownloadableUrl(Uri url, out uint downloadableId)
    {
        downloadableId = 0;

        if (url.Host is not "booth.pm") return false;

        if (url.Segments is not ["/", "downloadables/", string downloadableIdStr]) return false;

        if (downloadableIdStr[^1] is '/') downloadableIdStr = downloadableIdStr[..^1]; // Remove trailing '/' if present

        if (!uint.TryParse(downloadableIdStr, out downloadableId)) return false;

        return true;
    }
}
