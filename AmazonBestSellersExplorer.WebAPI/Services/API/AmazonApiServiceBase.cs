using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Dto;
using AmazonBestSellersExplorer.WebAPI.Models;

namespace AmazonBestSellersExplorer.WebAPI.Services.API
{
    public abstract class AmazonApiServiceBase : IAmazonApiService
    {
        public abstract Task<IEnumerable<ProductDto>> GetBestSellersAsync();

        protected IEnumerable<ProductDto> ParseAmazonResponse(string content)
        {
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            
            if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object && data.TryGetProperty("best_sellers", out var bestSellersArray) && bestSellersArray.ValueKind == JsonValueKind.Array)
            {
                var list = new List<ProductDto>();
                foreach (var item in bestSellersArray.EnumerateArray())
                {
                    list.Add(new ProductDto
                    {
                        Photo = GetString(item, "product_photo") ?? GetString(item, "product_image") ?? GetString(item, "image"),
                        Title = GetString(item, "product_title") ?? GetString(item, "title") ?? GetString(item, "name"),
                        Price = GetString(item, "product_price") ?? GetString(item, "price"),
                        StarRating = GetString(item, "product_star_rating") ?? GetString(item, "product_star") ?? GetString(item, "star_rating"),
                        Url = GetString(item, "product_url") ?? GetString(item, "url"),
                        Asin = GetString(item, "asin")
                    });
                }
                return list;
            }
            
            throw new InvalidOperationException("Unexpected API response structure");
        }

        private static string? GetString(JsonElement el, string propertyName)
        {
            if (el.ValueKind != JsonValueKind.Object) return null;
            if (el.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
            {
                try { return prop.GetString(); } catch { return prop.ToString(); }
            }
            return null;
        }
    }
}
