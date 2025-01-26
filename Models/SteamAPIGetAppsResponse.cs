using System.Text.Json.Serialization;

namespace NexAPI.Models
{
    public class SteamAPIGetAppsResponse
    {
        [JsonPropertyName("response")]
        public Response Response { get; set; } = new();
    }

    public class Response
    {
        [JsonPropertyName("apps")]
        public List<SteamGame> Apps { get; set; } = [];
        [JsonPropertyName("have_more_results")]
        public bool HaveMoreResults { get; set; }
        [JsonPropertyName("last_app_id")]
        public int LastAppId { get; set; }
    }
}
