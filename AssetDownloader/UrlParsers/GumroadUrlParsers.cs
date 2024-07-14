using System.Text.RegularExpressions;

namespace AssetDownloader.UrlParsers;

internal sealed partial class GumroadUrlParsers
{
    public static bool TryValidateAndParsePublicFileUrl(Uri url, out string publicFileId)
    {
        publicFileId = "";

        if (url.Host is not "public-files.gumroad.com") return false;

        if (url.Segments is not ["/", string id]) return false;

        if (id[^1] is '/') id = id[..^1]; // Remove trailing '/' if present

        // Check that the id is [a-zA-Z0-9]+
        if (!Regex.IsMatch(id, "^[a-zA-Z0-9]+$")) return false;

        publicFileId = id;

        return true;
    }

    internal static bool TryValidateAndParseCreatorUrl(Uri creatorUrl, out string creatorName)
    {
        creatorName = "";

        if (!creatorUrl.Host.EndsWith(".gumroad.com")) return false;

        creatorName = creatorUrl.Host[..^12];

        return creatorName.Length > 0;
    }
}
