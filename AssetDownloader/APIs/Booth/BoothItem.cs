namespace AssetDownloader.APIs.Booth;

public class BoothItem
{
    public string description { get; set; }
    public object factory_description { get; set; }
    public int id { get; set; }
    public bool is_adult { get; set; }
    public bool is_buyee_possible { get; set; }
    public bool is_end_of_sale { get; set; }
    public bool is_placeholder { get; set; }
    public bool is_sold_out { get; set; }
    public string name { get; set; }
    public string price { get; set; }
    public int? purchase_limit { get; set; }
    public string shipping_info { get; set; }
    public object small_stock { get; set; }
    public string url { get; set; }
    public string wish_list_url { get; set; }
    public int wish_lists_count { get; set; }
    public bool wished { get; set; }
    public object[] buyee_variations { get; set; }
    public Category category { get; set; }
    public object[] embeds { get; set; }
    public Image[] images { get; set; }
    public Order order { get; set; }
    public object gift { get; set; }
    public string report_url { get; set; }
    public Share share { get; set; }
    public Shop shop { get; set; }
    public object sound { get; set; }
    public Tag[] tags { get; set; }
    public Tag_Banners[] tag_banners { get; set; }
    public Tag_Combination tag_combination { get; set; }
    public object tracks { get; set; }
    public Variation[] variations { get; set; }
}

public class Category
{
    public int id { get; set; }
    public string name { get; set; }
    public Parent parent { get; set; }
    public string url { get; set; }
}

public class Parent
{
    public string name { get; set; }
    public string url { get; set; }
}

public class Order
{
    public string purchased_at { get; set; }
    public string url { get; set; }
}

public class Share
{
    public string[] hashtags { get; set; }
    public string text { get; set; }
}

public class Shop
{
    public string name { get; set; }
    public string subdomain { get; set; }
    public string thumbnail_url { get; set; }
    public string url { get; set; }
    public bool verified { get; set; }
}

public class Tag_Combination
{
    public string category { get; set; }
    public string tag { get; set; }
    public string url { get; set; }
}

public class Image
{
    public object caption { get; set; }
    public string original { get; set; }
    public string resized { get; set; }
}

public class Tag
{
    public string name { get; set; }
    public string url { get; set; }
}

public class Tag_Banners
{
    public string image_url { get; set; }
    public string name { get; set; }
    public string url { get; set; }
}

public class Variation
{
    public object buyee_html { get; set; }
    public Downloadable downloadable { get; set; }
    public object factory_image_url { get; set; }
    public bool has_download_code { get; set; }
    public int id { get; set; }
    public bool is_anshin_booth_pack { get; set; }
    public bool is_empty_allocatable_stock_with_preorder { get; set; }
    public bool is_empty_stock { get; set; }
    public bool is_factory_item { get; set; }
    public bool is_mailbin { get; set; }
    public bool is_waiting_on_arrival { get; set; }
    public string name { get; set; }
    public string order_url { get; set; }
    public int price { get; set; }
    public object small_stock { get; set; }
    public string status { get; set; }
    public string type { get; set; }
}

public class Downloadable
{
    public object[] musics { get; set; }
    public No_Musics[] no_musics { get; set; }
}

public class No_Musics
{
    public string file_name { get; set; }
    public string file_extension { get; set; }
    public string file_size { get; set; }
    public string name { get; set; }
    public string url { get; set; }
}
