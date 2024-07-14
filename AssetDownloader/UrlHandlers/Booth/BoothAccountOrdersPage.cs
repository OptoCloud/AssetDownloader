using AssetDownloader.DataExtractors.Booth;
using AssetDownloader.Extensions;
using AssetDownloader.HttpClients;
using AssetDownloader.UrlGenerators;
using System.Web;

namespace AssetDownloader.UrlHandlers.Booth;

public class BoothAccountOrdersPage : IUrlHandler
{
    private uint _page = 0;

    public bool TryValidateAndParseUrl(Uri url)
    {
        if (url.Host != "accounts.booth.pm" || url.Segments is not ["/", "orders" or "orders/"])
        {
            return false;
        }

        _page = url.GetQueryValue<uint>("page") ?? 0;

        return true;
    }

    public async Task<List<Uri>> Execute(FetchOptions fetchOptions)
    {
        Uri requestUrl = BoothUrlGenerators.GetAccountOrdersUrl(_page);

        using var client = BoothHttpClientFactory.CreateIndexHttpClient();
        using var response = await client.GetAsync(requestUrl);

        string str = await response.Content.ReadAsStringAsync();

        var scraper = new BoothAccountOrdersScraper();

        var ordersPage = scraper.Extract(str);
        if (ordersPage == null)
        {
            Console.WriteLine("Failed to parse account orders page");
            return [];
        }

        List<Uri> urls = [];

        foreach (var page in ordersPage.Pages)
        {
            urls.Add(page);
        }

        foreach (var order in ordersPage.Orders)
        {
            urls.Add(order.OrderUrl);
            urls.Add(order.ItemUrl);
            urls.Add(order.ItemJsonUrl);

            order.Deconstruct(out uint orderId, out uint itemId, out string itemVariantName, out DateTime orderCreatedAt);

            // TODO: Add order data to database
            //_relationItemToOrder[itemId] = orderId;
            Console.WriteLine($"Order: {orderId}, Item: {itemId}, Variant: {HttpUtility.HtmlDecode(itemVariantName)}, Created At: {orderCreatedAt}");
        }

        if (!fetchOptions.Crawl)
        {
            return [];
        }

        return urls;
    }
}
