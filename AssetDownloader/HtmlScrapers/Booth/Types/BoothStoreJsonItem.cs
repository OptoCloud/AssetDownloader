namespace AssetDownloader.HtmlScrapers.Booth.Types;

public class BoothStoreJsonItem
{
    public Category category { get; set; }
    public object _event { get; set; }
    public int id { get; set; }
    public bool is_adult { get; set; }
    public bool is_end_of_sale { get; set; }
    public bool is_placeholder { get; set; }
    public bool is_sold_out { get; set; }
    public bool is_vrchat { get; set; }
    public object minimum_stock { get; set; }
    public object music { get; set; }
    public string name { get; set; }
    public string price { get; set; }
    public Shop shop { get; set; }
    public string[] thumbnail_image_urls { get; set; }
    public string url { get; set; }
    public string shop_item_url { get; set; }
    public string wish_list_url { get; set; }
    public Tracking_Data tracking_data { get; set; }
}

public class Category
{
    public Name name { get; set; }
    public string url { get; set; }
}

public class Name
{
    public string en { get; set; }
    public string ja { get; set; }
}

public class Shop
{
    public string thumbnail_url { get; set; }
    public string name { get; set; }
    public string url { get; set; }
    public bool verified { get; set; }
}

public class Tracking_Data
{
    public int product_id { get; set; }
    public int product_price { get; set; }
    public string product_brand { get; set; }
    public int product_category { get; set; }
    public string tracking { get; set; }
}
