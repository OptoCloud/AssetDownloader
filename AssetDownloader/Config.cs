using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssetDownloader;

public sealed class Config
{
    private static readonly JsonSerializerOptions options = new()
    {
        WriteIndented = true
    };

    public static string Path { get; set; } = "./config.json";
    public static Config Instance { get; set; } = new();

    public static void Save()
    {
        File.WriteAllText(Path, JsonSerializer.Serialize(Instance, options));
    }

    public static void Load()
    {
        if (File.Exists(Path))
        {
            Instance = JsonSerializer.Deserialize<Config>(File.ReadAllText(Path), options) ?? throw new Exception("Failed to load config");
        }
        else
        {
            Instance = new Config();
            Save();
        }
    }

    [JsonPropertyName("booth_cookie")]
    public string? BoothCookie { get; set; } = null;

    [JsonPropertyName("gumroad_cookie")]
    public string? GumroadCookie { get; set; } = null;

    [JsonPropertyName("Auto_Zip")]
    public bool AutoZip { get; set; } = true;
}