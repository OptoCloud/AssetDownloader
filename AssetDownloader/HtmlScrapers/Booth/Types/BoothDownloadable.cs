using AssetDownloader.UrlGenerators;

namespace AssetDownloader.HtmlScrapers.Booth.Types;

public sealed record BoothDownloadable(uint DownloadableId, string DownloadableName)
{
    public Uri DownloadableUrl => BoothUrlGenerators.GetDownloadableUrl(DownloadableId);
}