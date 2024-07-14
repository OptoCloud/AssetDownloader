using AngleSharp.Common;
using AngleSharp.Dom;
using System.Web;

namespace AssetDownloader.DataExtractors;

internal static class Utils
{
    public static Uri? UrlFromHref(IElement? element, UriKind kind)
    {
        if (element is null)
        {
            Console.WriteLine("No element found");
            return null;
        }

        var href = element.GetAttribute("href");
        if (href is null)
        {
            Console.WriteLine("No href found");
            return null;
        }


        if (!Uri.TryCreate(href, kind, out Uri? url))
        {
            Console.WriteLine($"Invalid href: {href}");
            return null;
        }

        return url;
    }

    public static Dictionary<string, string?> QueryFromHref(IElement? element)
    {
        if (element is null) return [];

        var href = element.GetAttribute("href");
        if (href is null) return [];

        var parts = href.Split('?');
        if (parts.Length != 2) return [];

        var namedValues = HttpUtility.ParseQueryString(parts[1]);
        if (namedValues is null) return [];

        return namedValues.AllKeys.Where(x => x is not null).ToDictionary(x => x!, x => namedValues[x]);
    }

    public static uint PageFromHref(IElement? element, string querykey)
    {
        var query = QueryFromHref(element);
        if (query is null) return 1;

        var pageValue = query.GetValueOrDefault(querykey);
        if (pageValue is null) return 1;

        if (!uint.TryParse(pageValue, out uint page))
        {
            Console.WriteLine($"Invalid page value: {pageValue}");
            return 1;
        }

        return page;
    }

    public static Dictionary<string, string?>? DictionaryFromTable(IHtmlCollection<IElement> table)
    {
        if (table.Length % 2 != 0)
        {
            Console.WriteLine("Table has an odd number of elements");
            return null;
        }

        var dict = new Dictionary<string, string?>();

        for (int i = 0; i < table.Length; i += 2)
        {
            var key = table[i].TextContent.ToLower();
            var value = table[i + 1].TextContent;

            dict.Add(key, value);
        }

        return dict;
    }

    public static uint GetPageCountFromHrefQuery(IHtmlCollection<IElement> paginators, string querykey)
    {
        uint pageCount = 1;

        // Get all pages and select their query string
        foreach (var page in paginators)
        {
            if (page is null) continue;

            uint pageNumber = PageFromHref(page, querykey);

            if (pageNumber > pageCount) pageCount = pageNumber;
        }

        return pageCount;
    }
}
