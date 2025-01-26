using NexAPI.Models;
using System.Text;
using System.Text.Json;


namespace NexAPI.Services
{
    public class SteamAPIService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private const string STEAM_API_URL = "https://api.steampowered.com/IStoreService/GetAppList/v1/?include_games=true&include_dlc=false&include_software=false&include_videos=false&include_hardware=false";
        private readonly string STEAM_API_KEY;

        public SteamAPIService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));

            STEAM_API_KEY = _configuration["Steam:ApiKey"] ?? "";
        }

        public async Task<IEnumerable<SteamGame>> GetGamesAsync<IEnumerable>(int itemsPerPage)
        {
            var client = _httpClientFactory.CreateClient();
            string steamUrlWithKey = string.Concat(STEAM_API_URL, "&key=", STEAM_API_KEY, "&max_results=", itemsPerPage.ToString());
            HttpResponseMessage response = await client.GetAsync(steamUrlWithKey);

            try
            {
                Stream receiveStream = await response.Content.ReadAsStreamAsync();
                StreamReader receiveStreamReader = new(receiveStream, Encoding.UTF8);
                string text = receiveStreamReader.ReadToEnd();
                SteamAPIGetAppsResponse? apiResponse = JsonSerializer.Deserialize<SteamAPIGetAppsResponse>(text);

                if (apiResponse == null)
                    return [];

                List<SteamGame> result = apiResponse!.Response.Apps;
                return result;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"SteamUsersService Error in GetGamesAsync: {exception}");
                return [];
            }
        }

        public async Task<bool> VerifyOpenIdAsync(string openid)
        {
            string openIdValidationUrl = "https://steamcommunity.com/openid/login";
            var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsync(openIdValidationUrl, new FormUrlEncodedContent(new Dictionary<string, string>
        {
            { "openid.ns", "http://specs.openid.net/auth/2.0" },
            { "openid.mode", "check_authentication" },
            { "openid.return_to", openid }
        }));

            string content = await response.Content.ReadAsStringAsync();
            return content.Contains("is_valid:true");
        }

        public string GetSteamIdFromOpenIdAsync(string openid)
        {
            const string SteamIdPrefix = "https://steamcommunity.com/openid/id/";
            return openid.Replace(SteamIdPrefix, "");
        }

        public async Task<object> GetOwnedGamesAsync(string steamId)
        {
            string apiKey = _configuration["Steam:ApiKey"]!;
            string apiUrl = $"https://api.steampowered.com/IPlayerService/GetOwnedGames/v1/?key={apiKey}&steamid={steamId}&include_appinfo=true";

            var client = _httpClientFactory.CreateClient();
            var response = await client.GetStringAsync(apiUrl);
            return response; // Parse if necessary
        }
    }
}
