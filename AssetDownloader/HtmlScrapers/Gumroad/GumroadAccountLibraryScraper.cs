using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AssetDownloader.HtmlScrapers.Gumroad.Types;
using AssetDownloader.UrlGenerators;
using AssetDownloader.UrlParsers;
using System.Text.Json;

namespace AssetDownloader.HtmlScrapers.Gumroad;

internal sealed class GumroadAccountLibraryScraper
{
    public sealed record ProductCreator(string Name, string ProfileId, string ProfilePictureId)
    {
        public Uri CreatorUrl => GumroadUrlGenerators.GetCreatorUrl(Name);
        public Uri CreatorProfilePictureUrl => GumroadUrlGenerators.GetProfilePictureUrl(ProfilePictureId);
    }

    public sealed record ProductCover(string Id, string FileType, uint Height, uint Width)
    {
        public Uri CoverUrl => GumroadUrlGenerators.GetPublicFileUrl(Id);
    }

    public sealed record LibraryItem(ProductCreator Creator, string ItemId, string ThumnailId, string PurchaseId, string DownloadId, DateTime updatedAt, List<ProductCover> CoverIds)
    {
    }

    public sealed record Page(List<LibraryItem> Items, Dictionary<string, string> RawJSON)
    {
    }

    private ProductCreator? ExtractCreator(LibraryProductCreator? result)
    {
        if (result is null)
        {
            Console.WriteLine("No creator found");
            return null;
        }

        if (!Uri.TryCreate(result.profile_url, UriKind.Absolute, out Uri? creatorUrl))
        {
            Console.WriteLine("Creator url is not a valid url");
            return null;
        }

        if (!GumroadUrlParsers.TryValidateAndParseCreatorUrl(creatorUrl, out string? creatorName))
        {
            Console.WriteLine("Invalid creator url");
            return null;
        }

        if (!Uri.TryCreate(result.avatar_url, UriKind.Absolute, out Uri? avatarUrl))
        {
            Console.WriteLine("Creator avatar url is not a valid url");
            return null;
        }

        if (!GumroadUrlParsers.TryValidateAndParsePublicFileUrl(avatarUrl, out string? profilePictureId))
        {
            Console.WriteLine("Invalid creator avatar url");
            return null;
        }

        return new ProductCreator(result.name, creatorName, profilePictureId);
    }

    private ProductCover? ExtractCover(LibraryCover? cover)
    {
        if (cover is null)
        {
            Console.WriteLine("No cover found");
            return null;
        }

        if (!Uri.TryCreate(cover.url, UriKind.Absolute, out Uri? coverUrl))
        {
            Console.WriteLine("Cover url is not a valid url");
            return null;
        }

        if (!GumroadUrlParsers.TryValidateAndParsePublicFileUrl(coverUrl, out string? coverId))
        {
            Console.WriteLine("Invalid cover url");
            return null;
        }

        return new ProductCover(coverId, cover.filetype, cover.height, cover.width);
    }

    private LibraryItem? ExtractItem(LibraryResult? result)
    {
        if (result is null)
        {
            Console.WriteLine("No result found");
            return null;
        }

        var product = result.product;
        if (product is null)
        {
            Console.WriteLine("No product found");
            return null;
        }

        var creator = product.creator;
        if (creator is null)
        {
            Console.WriteLine("No creator found");
            return null;
        }

        ProductCreator? productCreator = ExtractCreator(creator);
        if (productCreator is null)
        {
            Console.WriteLine("Failed to extract creator");
            return null;
        }

        List<ProductCover> covers = [];

        foreach (var cover in product.covers)
        {
            ProductCover? productCover = ExtractCover(cover);
            if (productCover is null)
            {
                Console.WriteLine("Failed to extract cover");
                continue;
            }

            covers.Add(productCover);
        }

        return new LibraryItem(productCreator, product.permalink, product.thumbnail_url, product.permalink, product.permalink, product.updated_at, covers);
    }

    public Page? Extract(IHtmlDocument document)
    {
        var contextJson = document.QuerySelector("script[id*='js-react-on-rails-context']")?.TextContent;
        if (contextJson is null)
        {
            Console.WriteLine("No json context found");
            return null;
        }

        var jsonComponents = document.QuerySelectorAll("script[class*='js-react-on-rails-component']");
        if (jsonComponents is null)
        {
            Console.WriteLine("No json documents found");
            return null;
        }

        Dictionary<string, string> jsonDict = [];

        // Get all the component json
        foreach (var jsonComponent in jsonComponents)
        {
            var id = jsonComponent.GetAttribute("data-component-name");
            if (id is null)
            {
                Console.WriteLine("No component name found");
                continue;
            }

            var json = jsonComponent.TextContent;
            if (json is null)
            {
                Console.WriteLine("No json found");
                continue;
            }

            jsonDict[$"component-{id}"] = json;
        }

        var navJson = jsonDict.GetValueOrDefault("component-Nav");
        if (navJson is null)
        {
            Console.WriteLine("No nav json found");
            return null;
        }

        var libraryJson = jsonDict.GetValueOrDefault("component-LibraryPage");
        if (libraryJson is null)
        {
            Console.WriteLine("No library json found");
            return null;
        }

        var context = JsonSerializer.Deserialize<GumroadPageContext>(contextJson);
        if (context is null)
        {
            Console.WriteLine("Invalid context json");
            return null;
        }

        var nav = JsonSerializer.Deserialize<GumroadLibraryNav>(navJson);
        if (nav is null)
        {
            Console.WriteLine("Invalid nav json");
            return null;
        }

        var library = JsonSerializer.Deserialize<GumroadLibraryData>(libraryJson);
        if (library is null)
        {
            Console.WriteLine("Invalid library json");
            return null;
        }

        List<LibraryItem> items = [];

        foreach (var result in library.results)
        {
            LibraryItem? item = ExtractItem(result);
            if (item is null)
            {
                Console.WriteLine("Failed to extract item");
                continue;
            }

            items.Add(item);
        }

        return new Page(items, jsonDict);
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