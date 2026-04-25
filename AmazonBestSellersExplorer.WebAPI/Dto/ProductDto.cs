using System.Text.Json.Serialization;

namespace AmazonBestSellersExplorer.WebAPI.Models
{
    public class ProductDto
    {
        [JsonPropertyName("photo")]
        public string? Photo { get; set; }

        [JsonPropertyName("price")]
        public string? Price { get; set; }

        [JsonPropertyName("title")]
        public string? Title { get; set; }

        [JsonPropertyName("starRating")]
        public string? StarRating { get; set; }

        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("asin")]
        public string? Asin { get; set; }
    }
}

