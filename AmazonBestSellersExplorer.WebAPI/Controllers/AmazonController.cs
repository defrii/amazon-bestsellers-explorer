using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using AmazonBestSellersExplorer.WebAPI.Models;

namespace AmazonBestSellersExplorer.WebAPI.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AmazonController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
		: ControllerBase
	{
		// GET: api/amazon/best-sellers
		[HttpGet("best-sellers")]
		public async Task<IActionResult> GetBestSellers()
		{
			var useMock = configuration.GetValue<bool>("Amazon:UseMock");
			var mockFile = configuration["Amazon:MockFile"];

			try
			{
				string content;

				if (useMock)
				{
					if (string.IsNullOrWhiteSpace(mockFile))
					{
						return Problem(detail: "Amazon:MockFile is not configured.", statusCode: 500);
					}

					var mockPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", mockFile);
					if (!System.IO.File.Exists(mockPath))
					{
						return Problem(detail: $"Mock file not found: {mockPath}", statusCode: 500);
					}

					content = await System.IO.File.ReadAllTextAsync(mockPath);
				}
				else
				{
					var rapidHost = configuration["RapidApi:Host"];
					var rapidKey = configuration["RapidApi:Key"];
					if (string.IsNullOrEmpty(rapidHost) || string.IsNullOrEmpty(rapidKey))
					{
						return Problem(detail: "RapidApi:Host or RapidApi:Key is not configured.", statusCode: 500);
					}

					var apiUrl = $"https://{rapidHost}/best-sellers?category=software&type=BEST_SELLERS&page=1&country=PL";
					var client = httpClientFactory.CreateClient();
					using var request = new HttpRequestMessage(HttpMethod.Get, apiUrl);
					request.Headers.Add("x-rapidapi-host", rapidHost);
					request.Headers.Add("x-rapidapi-key", rapidKey);
					request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

					var response = await client.SendAsync(request);
					content = await response.Content.ReadAsStringAsync();

					if (!response.IsSuccessStatusCode)
					{
						return StatusCode((int)response.StatusCode, content);
					}
				}

				// Parse RapidAPI response and map to our DTO
				try
				{
					using var doc = JsonDocument.Parse(content);
					var root = doc.RootElement;
					JsonElement bestSellersArray;

					if (root.TryGetProperty("data", out var data) && data.ValueKind == JsonValueKind.Object && data.TryGetProperty("best_sellers", out bestSellersArray) && bestSellersArray.ValueKind == JsonValueKind.Array)
					{
						var list = new List<ProductDto>();
						foreach (var item in bestSellersArray.EnumerateArray())
						{
							string? photo = GetString(item, "product_photo") ?? GetString(item, "product_image") ?? GetString(item, "image");
							string? title = GetString(item, "product_title") ?? GetString(item, "title") ?? GetString(item, "name");
							string? price = GetString(item, "product_price") ?? GetString(item, "price");
							string? star = GetString(item, "product_star_rating") ?? GetString(item, "product_star") ?? GetString(item, "star_rating");
							string? productUrl = GetString(item, "product_url") ?? GetString(item, "url");
							string? asin = GetString(item, "asin");

							list.Add(new ProductDto
							{
								Photo = photo,
								Title = title,
								Price = price,
								StarRating = star,
								Url = productUrl,
								Asin = asin
							});
						}

						return Ok(list);
					}
					else
					{
						// Unexpected structure
						return Problem(detail: "Unexpected API response structure", statusCode: 502);
					}
				}
				catch (JsonException jex)
				{
					return Problem(detail: "Failed to parse API response: " + jex.Message, statusCode: 502);
				}
			}
			catch (Exception ex)
			{
				return Problem(detail: ex.Message, statusCode: 500);
			}
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
