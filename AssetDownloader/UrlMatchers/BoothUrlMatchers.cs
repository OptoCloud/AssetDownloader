using System.Text.RegularExpressions;

namespace AssetDownloader.UrlMatchers;

public sealed partial class BoothUrlMatchers
{
    public enum BoothUrlType
    {
        Unsupported,
        AccountOrdersPage,
        AccountLibraryPage,
        StorePage,
        ItemPage,
        ItemJsonDocument,
        OrderPage,

        /// <summary>
        /// Preffered over PximgUserImageUrl
        /// </summary>
        S2UserImageUrl,

        /// <summary>
        /// Preffered over PximgItemImageUrl
        /// </summary>
        S2ItemImageUrl,

        PximgUserImageUrl,
        PximgItemImageUrl,
        Downloadable,
    }

    private static BoothUrlType AccountOrderPageType(string orderId)
    {
        if (uint.TryParse(orderId, out _))
        {
            return BoothUrlType.OrderPage;
        }

        return BoothUrlType.Unsupported;
    }

    private static BoothUrlType WwwRegionItemPageType(string region, string item)
    {
        if (region is not ("en/" or "ja/" or "ko/" or "zh-cn/" or "zh-tw/"))
        {
            return BoothUrlType.Unsupported;
        }

        BoothUrlType type = BoothUrlType.ItemPage;

        if (item.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
        {
            type = BoothUrlType.ItemJsonDocument;
            item = item[..^5];
        }

        if (!uint.TryParse(item, out _))
        {
            return BoothUrlType.Unsupported;
        }

        return type;
    }

    private static BoothUrlType WwwRegionDownloadablePageType(string downloadableId)
    {
        if (!uint.TryParse(downloadableId, out _))
        {
            return BoothUrlType.Unsupported;
        }

        return BoothUrlType.Downloadable;
    }

    private static BoothUrlType CustomDomainRootPageType(string storeId)
    {
        if (storeId.Length == 0)
        {
            return BoothUrlType.Unsupported;
        }

        return BoothUrlType.StorePage;
    }

    [GeneratedRegex(@"^(\d+)x(\d+)(?:_a(\d+))?(?:_g(\d+))?/$")]
    private static partial Regex GetPximgSizeRegex();

    private static bool TryValidateImageName(string fullName)
    {
        if (fullName.Split('.') is not [string beforeDot, "jpg" or "png"])
        {
            return false;
        }

        if (beforeDot.Split('_') is not [string imageId, "base", "resized"])
        {
            return false;
        }

        return Guid.TryParse(imageId, out _);
    }

    private static BoothUrlType GetPximgUsersImagePageType(string dimensions, string someId, string imageName)
    {
        var dimensionsMatch = GetPximgSizeRegex().Match(dimensions);
        if (!dimensionsMatch.Success)
        {
            Console.WriteLine($"GetPximgUsersImagePageType: Unsupported dimensions: {dimensions}");
            return BoothUrlType.Unsupported;
        }

        if (!uint.TryParse(someId[..^1], out _))
        {
            Console.WriteLine($"GetPximgUsersImagePageType: Unsupported someId: {someId}");
            return BoothUrlType.Unsupported;
        }

        if (!TryValidateImageName(imageName))
        {
            Console.WriteLine($"GetPximgUsersImagePageType: Unsupported image name: {imageName}");
            return BoothUrlType.Unsupported;
        }

        return BoothUrlType.PximgUserImageUrl;
    }

    private static BoothUrlType GetBoothS2UserImagePageType(string someGuid, string imageName)
    {
        if (!Guid.TryParse(someGuid[..^1], out _))
        {
            Console.WriteLine($"GetBoothS2UserImagePageType: Unsupported someGuid: {someGuid}");
            return BoothUrlType.Unsupported;
        }

        if (!TryValidateImageName(imageName))
        {
            Console.WriteLine($"GetBoothS2UserImagePageType: Unsupported image name: {imageName}");
            return BoothUrlType.Unsupported;
        }

        return BoothUrlType.S2UserImageUrl;
    }

    private static BoothUrlType GetPximgItemsImagePageType(string someGuid, string someId, string imageName)
    {
        if (!Guid.TryParse(someGuid[..^1], out _))
        {
            Console.WriteLine($"GetPximgItemsImagePageType: Unsupported someGuid: {someGuid}");
            return BoothUrlType.Unsupported;
        }

        if (!uint.TryParse(someId[..^1], out _))
        {
            Console.WriteLine($"GetPximgItemsImagePageType: Unsupported someId: {someId}");
            return BoothUrlType.Unsupported;
        }

        if (!TryValidateImageName(imageName))
        {
            Console.WriteLine($"GetPximgItemsImagePageType: Unsupported image name: {imageName}");
            return BoothUrlType.Unsupported;
        }

        return BoothUrlType.PximgItemImageUrl;
    }

    private static BoothUrlType GetPximgItemsImagePageType(string dimensions, string someGuid, string someId, string imageName)
    {
        var dimensionsMatch = GetPximgSizeRegex().Match(dimensions);
        if (!dimensionsMatch.Success)
        {
            Console.WriteLine($"GetPximgItemsImagePageType: Unsupported dimensions: {dimensions}");
            return BoothUrlType.Unsupported;
        }

        if (!Guid.TryParse(someGuid[..^1], out _))
        {
            Console.WriteLine($"GetPximgItemsImagePageType: Unsupported someGuid: {someGuid}");
            return BoothUrlType.Unsupported;
        }

        if (!uint.TryParse(someId[..^1], out _))
        {
            Console.WriteLine($"GetPximgItemsImagePageType: Unsupported someId: {someId}");
            return BoothUrlType.Unsupported;
        }

        if (!TryValidateImageName(imageName))
        {
            Console.WriteLine($"GetPximgItemsImagePageType: Unsupported image name: {imageName}");
            return BoothUrlType.Unsupported;
        }

        return BoothUrlType.PximgItemImageUrl;
    }

    private static BoothUrlType GetBoothS2ItemImagePageType(string someGuid, string someId, string imageName)
    {
        if (!Guid.TryParse(someGuid[..^1], out _))
        {
            Console.WriteLine($"GetPximgBoothUsersIconImagePageType: Unsupported someGuid: {someGuid}");
            return BoothUrlType.Unsupported;
        }

        if (!uint.TryParse(someId[..^1], out _))
        {
            Console.WriteLine($"GetPximgBoothUsersIconImagePageType: Unsupported someId: {someId}");
            return BoothUrlType.Unsupported;
        }

        if (!TryValidateImageName(imageName))
        {
            Console.WriteLine($"GetPximgBoothUsersIconImagePageType: Unsupported image name: {imageName}");
            return BoothUrlType.Unsupported;
        }

        return BoothUrlType.S2ItemImageUrl;
    }

    private static BoothUrlType WwwPageType(Uri url)
    {
        return url.Segments switch
        {
        ["/", string region, "items/", string item] => WwwRegionItemPageType(region, item),
        ["/", "downloadables/", string downloadableId] => WwwRegionDownloadablePageType(downloadableId),
            _ => BoothUrlType.Unsupported,
        };
    }

    private static BoothUrlType GetBoothAccountPageType(Uri url)
    {
        return url.Segments switch
        {
        ["/", "orders/", string orderId] => AccountOrderPageType(orderId),
        ["/", "orders" or "orders/"] => BoothUrlType.AccountOrdersPage,
        ["/", "library" or "library/"] => BoothUrlType.AccountLibraryPage,
            _ => BoothUrlType.Unsupported,
        };
    }

    private static BoothUrlType GetBoothS2PageType(Uri url)
    {
        return url.Segments switch
        {
        ["/", "users/", string creatorS2Id, "icon_image/", string imageName] => GetBoothS2UserImagePageType(creatorS2Id, imageName),
        ["/", string someGuid, "i/", string someId, string imageName] => GetBoothS2ItemImagePageType(someGuid, someId, imageName),
            _ => BoothUrlType.Unsupported,
        };
    }

    private static BoothUrlType GetPximgBoothPageType(Uri url)
    {
        return url.Segments switch
        {
        ["/", string someGuid, "i/", string someId, string imageName] => GetPximgItemsImagePageType(someGuid, someId, imageName),
        ["/", "c/", string dimensions, "users/", string someId, "icon_image/", string imageName] => GetPximgUsersImagePageType(dimensions, someId, imageName),
        ["/", "c/", string dimensions, string someGuid, "i/", string someId, string imageName] => GetPximgItemsImagePageType(dimensions, someGuid, someId, imageName),
            _ => BoothUrlType.Unsupported,
        };
    }

    private static BoothUrlType GetBoothCustomPageType(Uri url)
    {
        if (!url.Host.EndsWith(".booth.pm")) return BoothUrlType.Unsupported;

        return url.Segments switch
        {
        ["/", "items" or "items/"] => BoothUrlType.ItemPage,
        ["/"] => CustomDomainRootPageType(url.Host[..^9]),
            _ => BoothUrlType.Unsupported,
        };
    }

    public static BoothUrlType GetBoothPageType(Uri url)
    {
        if (url.Scheme != "https") return BoothUrlType.Unsupported;

        return url.Host switch
        {
            "www.booth.pm" or "booth.pm" => WwwPageType(url),
            "accounts.booth.pm" => GetBoothAccountPageType(url),
            "s2.booth.pm" => GetBoothS2PageType(url),
            "booth.pximg.net" => GetPximgBoothPageType(url),
            _ => GetBoothCustomPageType(url),
        };
    }

    public static bool IsSupportedBoothPage(Uri url) => GetBoothPageType(url) != BoothUrlType.Unsupported;
}
