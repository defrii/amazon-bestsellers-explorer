using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;
using Microsoft.Extensions.Configuration;

namespace AmazonBestSellersExplorer.WebAPI.Services.API
{
    public class AmazonApiService : AmazonApiServiceBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AmazonApiService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public override async Task<IEnumerable<ProductDto>> GetBestSellersAsync()
        {
            var rapidHost = _configuration["RapidApi:Host"];
            var rapidKey = _configuration["RapidApi:Key"];
            if (string.IsNullOrEmpty(rapidHost) || string.IsNullOrEmpty(rapidKey))
            {
                throw new InvalidOperationException("RapidApi:Host or RapidApi:Key is not configured.");
            }

            var apiUrl = $"https://{rapidHost}/best-sellers?category=software&type=BEST_SELLERS&page=1&country=PL";
            var client = _httpClientFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
            request.Headers.Add("x-rapidapi-host", rapidHost);
            request.Headers.Add("x-rapidapi-key", rapidKey);
            request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"API request failed with status code {response.StatusCode}: {content}");
            }

            return ParseAmazonResponse(content);
        }
    }
}
