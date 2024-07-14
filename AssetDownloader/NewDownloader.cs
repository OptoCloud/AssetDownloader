using AssetDownloader.UrlHandlers;

namespace AssetDownloader;

public class NewDownloader
{
    private static List<Type> UrlHandlerTypes => AppDomain
        .CurrentDomain
        .GetAssemblies()
        .SelectMany(assembly => assembly.GetTypes())
        .Where(type => typeof(IUrlHandler).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
        .ToList();

    private class UrlHandlerCacheEntry
    {
        private Type _type;
        private IUrlHandler _handler;

        public UrlHandlerCacheEntry(Type type)
        {
            _type = type;
            _handler = (IUrlHandler?)Activator.CreateInstance(_type) ?? throw new InvalidOperationException("Failed to create instance of UrlHandler");
        }

        public bool TryMatchAndGetHandler(Uri uri, out IUrlHandler? handler)
        {
            if (!_handler.TryValidateAndParseUrl(uri))
            {
                handler = null;
                return false;
            }

            IUrlHandler executedHandler = _handler;

            // Replace with a fresh instance since the handler may have state
            _handler = (IUrlHandler?)Activator.CreateInstance(_type) ?? throw new InvalidOperationException("Failed to create instance of UrlHandler");

            handler = executedHandler;

            return true;
        }
    }

    private static readonly List<UrlHandlerCacheEntry> UrlHandlerCache = UrlHandlerTypes.Select(type => new UrlHandlerCacheEntry(type)).ToList();

    private static IUrlHandler? GetUrlHandler(Uri url)
    {
        lock (UrlHandlerCache)
        {
            foreach (var urlHandlerCacheEntry in UrlHandlerCache)
            {
                if (urlHandlerCacheEntry.TryMatchAndGetHandler(url, out var handler))
                {
                    return handler;
                }
            }
        }

        return null;
    }

    private static async Task<List<Uri>> ExecuteUrlHandler(Uri url, FetchOptions fetchOptions)
    {
        var handler = GetUrlHandler(url);
        if (handler == null)
        {
            Console.WriteLine($"Unsupported url: {url}");
            return [];
        }

        Console.WriteLine($"Scraping {url}");
        return await handler.Execute(fetchOptions);
    }

    private static readonly Dictionary<Uri, List<Uri>> _discoveredUrls = [];

    public static async Task Fetch(Uri url, FetchOptions? fetchOptions = null)
    {
        fetchOptions ??= new FetchOptions();

        Stack<List<Uri>> urls = new();
        urls.Push([url]);


        while (urls.Count != 0)
        {
            // Select all urls which haven't been discovered yet
            List<Uri> currentUrls = urls.Pop().Where(url => !_discoveredUrls.ContainsKey(url)).ToList();

            // Group and execute all tasks in parallel
            List<(Uri, Task<List<Uri>>)> pairs = currentUrls.Select(url => (url, ExecuteUrlHandler(url, fetchOptions))).ToList();

            // Await all tasks in parallel
            await Task.WhenAll(pairs.Select(pair => pair.Item2));

            // Add all discovered urls to the queue
            foreach (var (requestedUrl, task) in pairs)
            {
                var discoveredUrls = task.Result;

                _discoveredUrls[requestedUrl] = discoveredUrls;

                foreach (var discoveredUrl in discoveredUrls)
                {
                    urls.Push([discoveredUrl]);
                }
            }
        }
    }
}
