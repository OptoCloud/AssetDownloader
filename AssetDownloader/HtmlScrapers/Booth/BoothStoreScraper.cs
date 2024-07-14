using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AssetDownloader.HtmlScrapers.Booth.Types;
using AssetDownloader.UrlGenerators;
using System.Text.Json;
using System.Web;

namespace AssetDownloader.DataExtractors.Booth;

internal sealed class BoothStoreScraper
{
    public sealed record Page(string StoreId, string StoreName, string StoreDescription, List<BoothStoreJsonItem> Items, uint PageCount)
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

    public Page? Extract(IHtmlDocument document)
    {
        var shopHeadArea = document.QuerySelector("div[class=shop-head] > div[class=shop-head]");
        var contentArea = document.QuerySelector("main[role=main] > div[id=js-shop]");
        if (shopHeadArea is null || contentArea is null)
        {
            Console.WriteLine("No area of interest found");
            return null;
        }

        var storeId = shopHeadArea.QuerySelector("div[class=home-link-container__nickname] > a[title=Home]")?.TextContent?.ToLowerInvariant();
        if (storeId is null)
        {
            Console.WriteLine("No store id found");
            return null;
        }

        var storeName = shopHeadArea.QuerySelector("div[class=shop-name] span[class*=shop-name-label]")?.TextContent;
        if (storeName is null)
        {
            Console.WriteLine("No store name found");
            return null;
        }

        var storeDescription = shopHeadArea.QuerySelector("div[class=booth-description] > div[class*=autolink] div")?.TextContent;
        if (storeDescription is null)
        {
            Console.WriteLine("No store description found");
            return null;
        }

        var jsonItemValues = contentArea.QuerySelectorAll("li[class*=js-mount-point-shop-item-card]").Select(x => HttpUtility.HtmlDecode(x.GetAttribute("data-item"))).ToList();
        if (jsonItemValues.Any(x => x is null))
        {
            Console.WriteLine("Failed to extract json items");
            return null;
        }

        var jsonItems = jsonItemValues.Select(x => JsonSerializer.Deserialize<BoothStoreJsonItem>(x!)).ToList();
        if (jsonItems.Any(x => x is null))
        {
            Console.WriteLine("Failed to deserialize json items");
            return null;
        }

        var pages = Utils.GetPageCountFromHrefQuery(document.QuerySelectorAll("div[class=shop-pager] a[href*=items]"), "page");

        return new Page(storeId, storeName, storeDescription, jsonItems.Select(x => x!).ToList(), pages);
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
