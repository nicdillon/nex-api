using System.Text.Json.Serialization;

namespace NexAPI.Models
{
    public class SteamGame
    {
        [JsonPropertyName("appid")]
        public int AppId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("last_modified")]
        public long LastModified { get; set; }
        [JsonPropertyName("price_change_number")]
        public int PriceChangeNumber { get; set; }
    }
}
