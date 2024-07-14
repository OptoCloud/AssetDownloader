namespace AssetDownloader.UrlGenerators;

public static class GumroadUrlGenerators
{
    public static readonly Uri AccountLibraryUrl = new("https://app.gumroad.com/library");

    public static Uri GetPublicFileUrl(string publicFileId) => new($"https://public-files.gumroad.com/{publicFileId}");

    public static Uri GetCreatorUrl(string creatorName) => new($"https://{creatorName}.gumroad.com");
    public static Uri GetProfilePictureUrl(string publicFileId) => GetPublicFileUrl(publicFileId);
    public static Uri GetProductUrl(string productToken) => new($"https://app.gumroad.com/d/{productToken}");
    public static Uri GetProductFileUrl(string productToken, string fileId) => new($"https://app.gumroad.com/r/{productToken}/product_files?product_file_ids[]={fileId}");
}
