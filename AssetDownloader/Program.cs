using AssetDownloader;
using System.CommandLine;
using System.Reflection;

// Useful read: https://www.w3.org/TR/selectors-3/#selectors

Console.Title = $"AssetDownloader - V{Assembly.GetExecutingAssembly().GetName().Version}";

var rootCommand = new RootCommand("Asset Downloader");

var configOption = new Option<string>(
    name: "--config",
    description: "Path to configuration file",
    getDefaultValue: () => "./config.json"
);

var boothOption = new Option<string?>(
    name: "--booth",
    description: "Booth ID/URL"
);

var gumroadOption = new Option<string?>(
    name: "--gumroad",
    description: "Gumroad ID/URL"
);

var outputDirectoryOption = new Option<string>(
    name: "--output-dir",
    description: "Output Directory",
    getDefaultValue: () => "./assets"
);

rootCommand.AddGlobalOption(configOption);
rootCommand.AddOption(boothOption);
rootCommand.AddOption(outputDirectoryOption);

rootCommand.SetHandler(async (configFile, boothId, outputDirectory) =>
{
    Config.Path = configFile;
    Config.Load();

    if (string.IsNullOrEmpty(Config.Instance.BoothCookie))
    {
        Console.WriteLine("Please paste in your cookie from browser.\n");
        var cookie = Console.ReadLine();
        if (cookie == null)
        {
            Console.WriteLine("No cookie entered, exiting...");
            return;
        }

        Config.Instance.BoothCookie = cookie;

        Config.Save();

        Console.WriteLine("Cookie set!\n");
    }

    bool idFromArgument = true;
    if (boothId == null)
    {
        idFromArgument = false;
        Console.WriteLine("Enter the Booth ID or URL: ");
        boothId = Console.ReadLine();
        if (boothId == null)
        {
            Console.WriteLine("No Booth ID or URL entered, exiting...");
            return;
        }
    }

    // Check if the user entered a URL
    if (!Uri.TryCreate(boothId, UriKind.Absolute, out Uri? uri))
    {
        // If not, assume it's an ID
        throw new NotImplementedException();
    }

    await NewDownloader.Fetch(uri);
}, configOption, boothOption, outputDirectoryOption);

return await rootCommand.InvokeAsync(args);