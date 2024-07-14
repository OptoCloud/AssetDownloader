using System.Net.Http;
using System.Net;

namespace AssetDownloader.HttpClients;

public class ActiveLoginChecker
{
    public static async Task<bool> ValidateBoothCookie()
    {
        using var httpClient = BoothHttpClientFactory.CreateIndexHttpClient();
        using var response = await httpClient.GetAsync("https://accounts.booth.pm/settings");

        if (response.StatusCode == HttpStatusCode.OK)
        {
            return true;
        }

        if (response.StatusCode == HttpStatusCode.Redirect)
        {
            var locationHeader = response.Headers.Location;

            if (locationHeader is null)
            {
                Console.WriteLine("Invalid cookie, got redirected to unknown page");
                return false;
            }

            if (locationHeader.AbsolutePath == "/users/sign_in")
            {
                Console.WriteLine("Invalid cookie, got redirected to login page");
                return false;
            }
        }

        Console.WriteLine($"Invalid cookie, got status code {response.StatusCode}");

        return false;
    }
}
