using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AssetDownloader.HtmlScrapers.Booth.Types;
using AssetDownloader.UrlGenerators;
using AssetDownloader.UrlParsers;

namespace AssetDownloader.DataExtractors.Booth;

public sealed class BoothAccountLibraryScraper
{
    public sealed record LibraryItem(uint ItemId, string ItemName, string StoreId, string StoreName, List<BoothDownloadable> Downloads)
    {
        public Uri ItemUrl => BoothUrlGenerators.GetItemUrl(ItemId);
        public Uri ItemJsonUrl => BoothUrlGenerators.GetItemJsonUrl(ItemId);
        public Uri StoreUrl => BoothUrlGenerators.GetStoreUrl(StoreId);
    }
    public sealed record Page(List<LibraryItem> Items, uint PageCount)
    {
        public IEnumerable<Uri> Pages
        {
            get
            {
                for (uint i = 1; i <= PageCount; i++)
                {
                    yield return BoothUrlGenerators.GetAccountOrdersUrl(i);
                }
            }
        }
    }

    private BoothDownloadable? ExtractDownloadable(IElement element)
    {
        var downloadUrl = Utils.UrlFromHref(element.QuerySelector("a[href*=downloadables]"), UriKind.Absolute); // Example: https://booth.pm/downloadables/1234567
        if (downloadUrl is null)
        {
            Console.WriteLine("No download url found");
            return null;
        }

        var downloadName = element.QuerySelector("div[class*=min-w-0] div[class*=typography-]")?.TextContent; // Example: "Download Name"
        if (downloadName is null)
        {
            Console.WriteLine("No download name found");
            return null;
        }

        if (!BoothUrlParsers.TryValidateAndParseDownloadableUrl(downloadUrl, out uint downloadableId))
        {
            Console.WriteLine($"Invalid downloadable url: {downloadUrl}");
            return null;
        }

        return new BoothDownloadable(downloadableId, downloadName);
    }

    private LibraryItem? ExtractItem(IElement element)
    {
        var areaOfInterest = element.QuerySelector("div[class*=flex]");
        if (areaOfInterest is null)
        {
            Console.WriteLine("No area of interest found");
            return null;
        }

        var itemUrl = Utils.UrlFromHref(areaOfInterest.QuerySelector("a[href*=items]"), UriKind.Absolute); // Example: https://booth.pm/en/items/1234567
        if (itemUrl is null)
        {
            Console.WriteLine("No item url found");
            return null;
        }

        var itemName = areaOfInterest.QuerySelector("div[class*=text-text-default]")?.TextContent; // Example: "Item Name"
        if (itemName is null)
        {
            Console.WriteLine("No item name found");
            return null;
        }

        var authorUrlElement = areaOfInterest.QuerySelector("div[class*=text-text-default] + a[href*=booth]");

        var authorUrl = Utils.UrlFromHref(authorUrlElement, UriKind.Absolute); // Example: https://author.booth.pm/
        if (authorUrl is null)
        {
            Console.WriteLine("No author url found");
            return null;
        }

        var authorName = authorUrlElement!.QuerySelector("div[class*=typography-]")?.TextContent; // Example: "Author Name"
        if (authorName is null)
        {
            Console.WriteLine("No author name found");
            return null;
        }

        var downloads = element.QuerySelectorAll("div[class*=mt-16] div[class*=mt-16]") // Example: "Download Name"
            .Select(ExtractDownloadable)
            .ToList();

        if (downloads.Any(download => download is null))
        {
            Console.WriteLine("Failed to extract downloads");
            return null;
        }

        // Sanity check and extract the item id
        if (!BoothUrlParsers.TryValidateAndParseItemUrl(itemUrl, out uint itemId))
        {
            Console.WriteLine($"Invalid item url: {itemUrl}");
            return null;
        }

        // Sanity check and extract the author id
        if (!BoothUrlParsers.TryValidateAndParseStoreUrl(authorUrl, out var authorId, out _))
        {
            Console.WriteLine($"Invalid author url: {authorUrl}");
            return null;
        }

        return new LibraryItem(itemId, itemName, authorId, authorName, downloads.Select(download => download!).ToList());
    }

    public Page? Extract(IHtmlDocument document)
    {
        var areaOfInterest = document.QuerySelector("main[role=main]");
        if (areaOfInterest is null)
        {
            Console.WriteLine("No area of interest found");
            return null;
        }

        var items = areaOfInterest.QuerySelectorAll("div[class*=w-full] > div[class*=mb-]") // Example: "Item Name"
            .Select(ExtractItem)
            .ToList();

        if (items.Any(item => item is null))
        {
            Console.WriteLine("Failed to extract items");
            return null;
        }

        var pager = areaOfInterest.QuerySelector("div[class*=pager]");
        if (pager is null)
        {
            Console.WriteLine("No pager found");
            return null;
        }

        uint pageCount = Utils.GetPageCountFromHrefQuery(pager.QuerySelectorAll("li a[href*=library]"), "page");

        return new Page(items.Select(item => item!).ToList(), pageCount);
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