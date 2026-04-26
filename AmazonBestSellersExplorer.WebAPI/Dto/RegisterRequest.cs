using System.Text.Json.Serialization;

namespace AmazonBestSellersExplorer.WebAPI.Dto
{
    public class RegisterRequest
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;

        [JsonPropertyName("password")]
        public string Password { get; set; } = string.Empty;
    }
}

