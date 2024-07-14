namespace AssetDownloader.UrlHandlers;

public interface IUrlHandler
{
    bool TryValidateAndParseUrl(Uri url);

    Task<List<Uri>> Execute(FetchOptions fetchOptions);
}
