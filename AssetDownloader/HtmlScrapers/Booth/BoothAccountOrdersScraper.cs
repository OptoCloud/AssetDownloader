using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AssetDownloader.Extensions;
using AssetDownloader.UrlGenerators;
using AssetDownloader.UrlParsers;

namespace AssetDownloader.DataExtractors.Booth;

public sealed class BoothAccountOrdersScraper
{
    public sealed record Order(uint OrderId, uint ItemId, string ItemVariantName, DateTime OrderCreatedAt)
    {
        public Uri ItemUrl => BoothUrlGenerators.GetItemUrl(ItemId);
        public Uri ItemJsonUrl => BoothUrlGenerators.GetItemJsonUrl(ItemId);
        public Uri OrderUrl => BoothUrlGenerators.GetAccountOrderUrl(OrderId);
    }
    public sealed record Page(List<Order> Orders, uint PageCount)
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

    private Order? ExtractOrder(IElement element)
    {
        var itemUrl = Utils.UrlFromHref(element.QuerySelector("a[href*=items]"), UriKind.Absolute); // Example: https://booth.pm/en/items/1234567
        if (itemUrl is null)
        {
            Console.WriteLine("No item url found");
            return null;
        }

        var orderUrl = element.QuerySelector("a[href*=orders]")?.GetAttribute("href"); // Example: /orders/1234567
        if (orderUrl is null)
        {
            Console.WriteLine("No order url found");
            return null;
        }

        var name = element.QuerySelector("div[class*=u-tpg-caption1]")?.TextContent; // Example: Item Name
        if (name is null)
        {
            Console.WriteLine("No name found");
            return null;
        }

        var createdAt = element.QuerySelector("div[class*=u-tpg-caption2]")?.TextContent; // Example: Created At: 2021/01/01 00:00:00
        if (createdAt is null)
        {
            Console.WriteLine("No created at found");
            return null;
        }

        // Sanity check the url's
        if (!BoothUrlParsers.TryValidateAndParseItemUrl(itemUrl, out uint itemId))
        {
            Console.WriteLine($"Invalid item url: {itemUrl}");
            return null;
        }

        if (!uint.TryParse(orderUrl.RemoveStart("/orders/"), out var orderId))
        {
            Console.WriteLine($"Invalid order id: {orderUrl}");
            return null;
        }

        // Remove "Created At: " from the string, case insensitive
        createdAt = createdAt.Replace("created at: ", "", StringComparison.OrdinalIgnoreCase);

        // Parse the date
        if (!DateTime.TryParse(createdAt, out var date))
        {
            Console.WriteLine($"Failed to parse date: {createdAt}");
            return null;
        }

        return new Order(orderId, itemId, name, date);
    }

    public Page? Extract(IHtmlDocument document)
    {
        var areaOfInterest = document.QuerySelectorAll("div[class=manage-page-body] > div[class*=container]").SingleOrDefault();
        if (areaOfInterest is null)
        {
            Console.WriteLine("No area of interest found");
            return null;
        }

        var orders = areaOfInterest.QuerySelectorAll("div[class*=l-orders-index] > div[class*=sheet]").Select(x => ExtractOrder(x)).ToList();
        if (orders.Count == 0)
        {
            Console.WriteLine("No orders found");
            return null;
        }
        if (orders.Any(x => x is null))
        {
            Console.WriteLine("Failed to extract all orders");
            return null;
        }

        var pager = areaOfInterest.QuerySelectorAll("div[class*=pager] > nav > ul").SingleOrDefault();
        if (pager is null)
        {
            Console.WriteLine("No pager found");
            return null;
        }

        uint pageCount = Utils.GetPageCountFromHrefQuery(pager.QuerySelectorAll("li a[href*=orders]"), "page");

        return new Page(orders.Select(x => x!).ToList(), pageCount);
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
