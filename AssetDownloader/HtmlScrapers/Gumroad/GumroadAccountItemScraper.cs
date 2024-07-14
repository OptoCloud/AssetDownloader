using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AssetDownloader.HtmlScrapers.Gumroad.Types;
using AssetDownloader.UrlGenerators;
using AssetDownloader.UrlParsers;
using System.Text.Json;

namespace AssetDownloader.HtmlScrapers.Gumroad;

public sealed class GumroadAccountItemScraper
{
    public sealed record ProductCreator(string Name, string ProfilePicture_PublicFileId)
    {
        public Uri CreatorUrl => GumroadUrlGenerators.GetCreatorUrl(Name);
        public Uri CreatorProfilePictureUrl => GumroadUrlGenerators.GetProfilePictureUrl(ProfilePicture_PublicFileId);
    }

    public sealed record ProductFile(string ProductToken, string FileId, string FileName, string Description, long FileSize, string FileExtension)
    {
        public Uri FileUrl => GumroadUrlGenerators.GetProductFileUrl(ProductToken, FileId);
    }

    public sealed record Page(ProductCreator Creator, string ProductToken, string? ProductLicense, List<ProductFile> ProductFiles, Dictionary<string, string> RawJSON)
    {
        public Uri ProductUrl => GumroadUrlGenerators.GetProductUrl(ProductToken);
    }

    private ProductCreator? ExtractCreator(ItemCreator? productItemCreator)
    {
        if (productItemCreator is null)
        {
            Console.WriteLine("No creator found");
            return null;
        }

        if (!Uri.TryCreate(productItemCreator.avatar_url, UriKind.Absolute, out Uri? avatarUrl))
        {
            Console.WriteLine("Creator avatar url is not a valid url");
            return null;
        }

        if (!GumroadUrlParsers.TryValidateAndParsePublicFileUrl(avatarUrl, out string? publicFileId))
        {
            Console.WriteLine("Invalid creator avatar url");
            return null;
        }

        return new ProductCreator(productItemCreator.name, publicFileId);
    }

    public Page? Extract(IHtmlDocument document)
    {
        var pageContextJson = document.QuerySelector("script[id*='js-react-on-rails-context']")?.TextContent;
        if (pageContextJson is null)
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

            Console.WriteLine($"Found component: {id}");

            jsonDict[$"component-{id}"] = json;
        }

        var downloadJson = jsonDict.GetValueOrDefault("component-DownloadPageWithContent");
        if (downloadJson is null)
        {
            Console.WriteLine("No library json found");
            return null;
        }

        var context = JsonSerializer.Deserialize<GumroadPageContext>(pageContextJson);
        if (context is null)
        {
            Console.WriteLine("Invalid context json");
            return null;
        }

        var pageContent = JsonSerializer.Deserialize<GumroadItemData>(downloadJson);
        if (pageContent is null)
        {
            Console.WriteLine("Invalid item json");
            return null;
        }

        ProductCreator? creator = ExtractCreator(pageContent.creator);
        if (creator is null)
        {
            Console.WriteLine("Failed to extract creator");
            return null;
        }

        List<ProductFile> productFiles = [];

        foreach (var item in pageContent.content.content_items)
        {
            productFiles.Add(new ProductFile(
                pageContent.token,
                item.id,
                item.file_name,
                item.description,
                item.file_size,
                item.extension.ToLowerInvariant()
            ));
        }

        return new Page(creator, pageContent.token, pageContent.content.license?.license_key, productFiles, jsonDict);
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