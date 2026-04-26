using System;
using System.Text.Json.Serialization;

namespace AmazonBestSellersExplorer.WebAPI.Dto
{
    public class JwtTokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; } = string.Empty;

        [JsonPropertyName("expires")]
        public DateTime Expires { get; set; }
    }
}
