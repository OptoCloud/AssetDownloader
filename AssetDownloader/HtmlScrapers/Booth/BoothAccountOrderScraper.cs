using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AssetDownloader.HtmlScrapers.Booth.Types;
using AssetDownloader.UrlGenerators;
using AssetDownloader.UrlParsers;

namespace AssetDownloader.DataExtractors.Booth;

public sealed class BoothAccountOrderScraper
{
    public sealed record ItemVariant(uint ItemId, string ItemVariantName, List<BoothDownloadable> ItemDownloadables)
    {
        public Uri ItemUrl => BoothUrlGenerators.GetItemUrl(ItemId);
        public Uri ItemJsonUrl => BoothUrlGenerators.GetItemJsonUrl(ItemId);
    }
    public sealed record Page(uint OrderId, string StoreId, DateTime OrderCreatedAt, List<ItemVariant> OrderItemVariants)
    {
        public Uri OrderUrl => BoothUrlGenerators.GetAccountOrderUrl(OrderId);
    }

    private BoothDownloadable? ExtractDownloadable(IElement element)
    {
        var downloadName = element.QuerySelector("div[class*=u-flex-1] > b")?.TextContent; // Example: "Download Name"
        if (downloadName is null)
        {
            Console.WriteLine("No download name found");
            return null;
        }

        var downloadUrl = Utils.UrlFromHref(element.QuerySelector("div[class*=u-flex-none] a[href*=downloadables]"), UriKind.Absolute); // Example: https://booth.pm/downloadables/1234567
        if (downloadUrl is null)
        {
            Console.WriteLine("No download url found");
            return null;
        }

        // Sanity check the url
        if (!BoothUrlParsers.TryValidateAndParseDownloadableUrl(downloadUrl, out uint downloadableId))
        {
            Console.WriteLine($"Invalid download url: {downloadUrl}");
            return null;
        }

        return new BoothDownloadable(downloadableId, downloadName);
    }

    private ItemVariant? ExtractItemVariant(IElement element)
    {
        var title = element.QuerySelector("div[class*=u-flex-1] div[class*=u-tpg-title4] > b > a[href]")?.TextContent; // Example: Item Variant Name
        if (title is null)
        {
            Console.WriteLine("No title found");
            return null;
        }

        var itemsUrl = Utils.UrlFromHref(element.QuerySelector("div[class*=u-d-flex] > div[class*=u-flex-none] > a[href*=items]"), UriKind.Absolute); // Example: https://store.booth.pm/items/1234567
        if (itemsUrl is null)
        {
            Console.WriteLine("No order item info element found");
            return null;
        }

        // Sanity check the url
        if (!BoothUrlParsers.TryValidateAndParseItemUrl(itemsUrl, out uint itemId))
        {
            Console.WriteLine($"Invalid item url: {itemsUrl}");
            return null;
        }

        var itemDownloadables = element.QuerySelectorAll("div[class*=list] > div[class=legacy-list-item]").Select(ExtractDownloadable).ToList();
        if (itemDownloadables.Any(x => x is null))
        {
            Console.WriteLine("Failed to extract item downloadables");
            return null;
        }

        return new ItemVariant(itemId, title, itemDownloadables.Select(x => x!).ToList());
    }

    public Page? Extract(IHtmlDocument document)
    {
        var orderInfo = Utils.DictionaryFromTable(document.QuerySelectorAll("div[class*=u-mt-500] > div[class*=l-row] > div[class*=l-col-pc-]"));
        if (orderInfo is null)
        {
            Console.WriteLine("Failed to extract order info");
            return null;
        }
        var createdAtStr = orderInfo["created at"];
        var orderIdStr = orderInfo["order number"];

        if (createdAtStr is null || orderIdStr is null)
        {
            Console.WriteLine("Failed to extract order info");
            return null;
        }

        if (!DateTime.TryParse(createdAtStr, out var createdAt))
        {
            Console.WriteLine($"Invalid created at: {createdAtStr}");
            return null;
        }

        if (!uint.TryParse(orderIdStr, out var orderId))
        {
            Console.WriteLine($"Invalid order id: {orderIdStr}");
            return null;
        }

        var itemDetailsArea = document.QuerySelector("div[class=l-order-detail-by-shop]");
        if (itemDetailsArea is null)
        {
            Console.WriteLine("No area of interest found");
            return null;
        }

        var storeId = itemDetailsArea.QuerySelector("div[class*=l-order-detail-sheet-group-header] > b > a[href*=booth]")?.TextContent; // Example: "Store Name"
        if (storeId is null)
        {
            Console.WriteLine("No store id found");
            return null;
        }

        var variants = itemDetailsArea.QuerySelectorAll("div[class*=sheet-group] > div[class*=mobile]").Select(ExtractItemVariant).ToList();

        if (variants.Any(x => x is null))
        {
            Console.WriteLine("Failed to extract item variants");
            return null;
        }

        return new Page(orderId, storeId, createdAt, variants.Select(x => x!).ToList());
    }
    public Page? Extract(Stream htmlStream)
    {
        HtmlParser parser = new();
        return Extract(parser.ParseDocument(htmlStream));
    }
    public Page? Extract(string html)
    {
        HtmlParser parser = new();
        return Extract(parser.ParseDocument(html));
    }
}