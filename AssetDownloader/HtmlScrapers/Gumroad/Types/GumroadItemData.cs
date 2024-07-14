namespace AssetDownloader.HtmlScrapers.Gumroad.Types;


public class GumroadLibraryData
{
    public LibraryResult[] results { get; set; }
    public LibraryCreator[] creators { get; set; }
    public object[] bundles { get; set; }
    public bool wishlists_enabled { get; set; }
}

public class LibraryResult
{
    public LibraryProduct product { get; set; }
    public LibraryPurchase purchase { get; set; }
}

public class LibraryProduct
{
    public string name { get; set; }
    public string creator_id { get; set; }
    public LibraryProductCreator creator { get; set; }
    public string thumbnail_url { get; set; }
    public string native_type { get; set; }
    public bool is_bundle { get; set; }
    public LibraryCover[] covers { get; set; }
    public string main_cover_id { get; set; }
    public DateTime updated_at { get; set; }
    public string permalink { get; set; }
    public bool has_third_party_analytics { get; set; }
}

public class LibraryProductCreator
{
    public string name { get; set; }
    public string profile_url { get; set; }
    public string avatar_url { get; set; }
}

public class LibraryCover
{
    public string url { get; set; }
    public string original_url { get; set; }
    public string thumbnail { get; set; }
    public string id { get; set; }
    public string type { get; set; }
    public string filetype { get; set; }
    public uint width { get; set; }
    public uint height { get; set; }
    public float native_width { get; set; }
    public float native_height { get; set; }
}

public class LibraryPurchase
{
    public string id { get; set; }
    public string email { get; set; }
    public bool is_archived { get; set; }
    public string download_url { get; set; }
    public string variants { get; set; }
    public object bundle_id { get; set; }
}

public class LibraryCreator
{
    public string id { get; set; }
    public string name { get; set; }
    public int count { get; set; }
}
