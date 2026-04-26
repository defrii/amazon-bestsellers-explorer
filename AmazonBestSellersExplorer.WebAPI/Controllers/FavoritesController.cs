using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AmazonBestSellersExplorer.WebAPI.Helpers;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Services;
using System.Security.Claims;
using System.Globalization;
using System.Linq;
using AmazonBestSellersExplorer.WebAPI.Dto;

namespace AmazonBestSellersExplorer.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteProductService _service;

        public FavoritesController(IFavoriteProductService service)
        {
            _service = service;
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var favorites = await _service.GetFavoriteAsinsAsync(userId.Value);
            return Ok(favorites);
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetFavoriteDetails()
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var favorites = await _service.GetFavoriteDetailsAsync(userId.Value);

            var dtos = favorites.Select(f => new ProductDto
            {
                Asin = f.Asin,
                Title = f.Title,
                Price = f.Price > 0 ? f.Price.ToString("F2", CultureInfo.InvariantCulture) : null,
                Url = f.Url,
                Photo = f.Photo,
                StarRating = f.StarRating.HasValue ? f.StarRating.Value.ToString(CultureInfo.InvariantCulture) : null
            }).ToList();

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] ProductDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            if (string.IsNullOrWhiteSpace(dto.Asin) || string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("ASIN and Title are required.");

            decimal parsedPrice = 0;
            if (!string.IsNullOrWhiteSpace(dto.Price))
            {
                var priceString = new string(dto.Price.Where(c => char.IsDigit(c) || c == '.' || c == ',').ToArray());
                priceString = priceString.Replace(',', '.');
                decimal.TryParse(priceString, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedPrice);
            }

            double parsedRating = 0;
            if (!string.IsNullOrWhiteSpace(dto.StarRating))
            {
                var numberStr = ValidationHelper.ExtractFirstNumber(dto.StarRating);
                if (numberStr != null)
                    double.TryParse(numberStr, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedRating);
            }

            var favorite = new FavoriteProduct
            {
                UserId = userId.Value,
                Asin = dto.Asin,
                Title = dto.Title,
                Price = parsedPrice,
                Url = dto.Url,
                Photo = dto.Photo,
                StarRating = parsedRating > 0 ? parsedRating : null
            };

            var result = await _service.AddFavoriteAsync(favorite);
            if (!result.IsSuccess)
                return Conflict(result.ErrorMessage);

            return Ok(new { asin = result.Value!.Asin });
        }

        [HttpDelete("{asin}")]
        public async Task<IActionResult> RemoveFavorite(string asin)
        {
            var userId = GetCurrentUserId();
            if (userId is null) return Unauthorized();

            var result = await _service.RemoveFavoriteAsync(userId.Value, asin);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return NoContent();
        }
    }
}
