using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AmazonBestSellersExplorer.WebAPI.Helpers;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Services;
using System.Security.Claims;
using System.Globalization;
using System.Linq;

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

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException();
        }

        [HttpGet]
        public async Task<IActionResult> GetFavorites()
        {
            try
            {
                var userId = GetCurrentUserId();
                var favorites = await _service.GetFavoriteAsinsAsync(userId);

                return Ok(favorites);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetFavoriteDetails()
        {
            try
            {
                var userId = GetCurrentUserId();
                var favorites = await _service.GetFavoriteDetailsAsync(userId);
                
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
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] ProductDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

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
                    UserId = userId,
                    Asin = dto.Asin,
                    Title = dto.Title,
                    Price = parsedPrice,
                    Url = dto.Url,
                    Photo = dto.Photo,
                    StarRating = parsedRating > 0 ? parsedRating : null
                };

                try
                {
                    var result = await _service.AddFavoriteAsync(favorite);
                    return Ok(new { asin = result.Asin });
                }
                catch (System.InvalidOperationException ex)
                {
                    return Conflict(ex.Message);
                }
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        [HttpDelete("{asin}")]
        public async Task<IActionResult> RemoveFavorite(string asin)
        {
            try
            {
                var userId = GetCurrentUserId();

                var success = await _service.RemoveFavoriteAsync(userId, asin);

                if (!success)
                    return NotFound();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
