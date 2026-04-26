using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Dto;
using AmazonBestSellersExplorer.WebAPI.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace AmazonBestSellersExplorer.WebAPI.Services.API
{
    public class AmazonApiService : AmazonApiServiceBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;
        private const string CacheKey = "AmazonBestSellers";
        private const int DefaultCacheMinutes = 5;

        public AmazonApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IMemoryCache cache)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _cache = cache;
        }

        public override async Task<IEnumerable<ProductDto>> GetBestSellersAsync()
        {
            var useCache = _configuration.GetValue<bool>("Amazon:UseCache", true);

            if (useCache && _cache.TryGetValue(CacheKey, out IEnumerable<ProductDto> cachedProducts))
            {
                return cachedProducts;
            }

            var rapidHost = _configuration["RapidApi:Host"];
            var rapidKey = _configuration["RapidApi:Key"];
            if (string.IsNullOrEmpty(rapidHost) || string.IsNullOrEmpty(rapidKey))
            {
                throw new InvalidOperationException("RapidApi:Host lub RapidApi:Key nie zostały skonfigurowane.");
            }

            var apiUrl = $"https://{rapidHost}/best-sellers?category=software&type=BEST_SELLERS&country=PL";
            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("x-rapidapi-host", rapidHost);
            request.Headers.Add("x-rapidapi-key", rapidKey);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Zapytanie API nie powiodło się z kodem błędu {response.StatusCode}: {content}");
            }

            var result = ParseAmazonResponse(content);

            if (useCache)
            {
                var cacheMinutes = _configuration.GetValue<int>("Amazon:CacheMinutes", DefaultCacheMinutes);
                _cache.Set(CacheKey, result, TimeSpan.FromMinutes(cacheMinutes));
            }

            return result;
        }
    }
}
