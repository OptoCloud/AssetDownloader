namespace AssetDownloader.HtmlScrapers.Gumroad.Types;



public class GumroadItemData
{
    public ItemContent content { get; set; }
    public bool product_has_third_party_analytics { get; set; }
    public string terms_page_url { get; set; }
    public string token { get; set; }
    public ItemCreator creator { get; set; }
    public object installment { get; set; }
    public ItemPurchase purchase { get; set; }
    public bool is_mobile_app_web_view { get; set; }
    public object content_unavailability_reason_code { get; set; }
    public string add_to_library_option { get; set; }
}

public class ItemContent
{
    public ItemLicense license { get; set; }
    public ItemContent_Items[] content_items { get; set; }
    public ItemRich_Content_Pages[] rich_content_pages { get; set; }
    public object[] posts { get; set; }
    public object video_transcoding_info { get; set; }
    public object custom_receipt { get; set; }
    public object discord { get; set; }
    public string ios_app_url { get; set; }
    public string android_app_url { get; set; }
    public bool can_open_in_app { get; set; }
    public object download_all_button { get; set; }
}

public class ItemLicense
{
    public string license_key { get; set; }
    public bool is_multiseat_license { get; set; }
    public int seats { get; set; }
}

public class ItemContent_Items
{
    public string type { get; set; }
    public string file_name { get; set; }
    public string description { get; set; }
    public string extension { get; set; }
    public long file_size { get; set; }
    public object pagelength { get; set; }
    public object duration { get; set; }
    public string id { get; set; }
    public string download_url { get; set; }
    public object stream_url { get; set; }
    public object media_params { get; set; }
    public object kindle_data { get; set; }
    public object latest_media_location { get; set; }
    public object content_length { get; set; }
    public object read_url { get; set; }
    public object external_link_url { get; set; }
    public object[] subtitle_files { get; set; }
    public bool pdf_stamp_enabled { get; set; }
    public bool processing { get; set; }
    public object thumbnail_url { get; set; }
}

public class ItemRich_Content_Pages
{
    public string page_id { get; set; }
    public object title { get; set; }
    public object variant_id { get; set; }
    public ItemDescription description { get; set; }
    public DateTime updated_at { get; set; }
}

public class ItemDescription
{
    public string type { get; set; }
    public ItemContent1[] content { get; set; }
}

public class ItemContent1
{
    public string type { get; set; }
    public ItemAttrs attrs { get; set; }
}

public class ItemAttrs
{
    public string id { get; set; }
    public string uid { get; set; }
}

public class ItemCreator
{
    public string name { get; set; }
    public string profile_url { get; set; }
    public string avatar_url { get; set; }
}

public class ItemPurchase
{
    public string id { get; set; }
    public object bundle_purchase_id { get; set; }
    public string email { get; set; }
    public DateTime created_at { get; set; }
    public bool is_archived { get; set; }
    public string product_permalink { get; set; }
    public string product_id { get; set; }
    public string product_name { get; set; }
    public object variant_id { get; set; }
    public object variant_name { get; set; }
    public string product_long_url { get; set; }
    public ItemRating rating { get; set; }
    public object membership { get; set; }
}

public class ItemRating
{
    public object current_rating { get; set; }
}
