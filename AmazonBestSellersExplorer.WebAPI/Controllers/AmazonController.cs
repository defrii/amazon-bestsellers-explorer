using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AmazonBestSellersExplorer.WebAPI.Helpers;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Services;
using AmazonBestSellersExplorer.WebAPI.Services.API;
using AmazonBestSellersExplorer.WebAPI.Dto;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AmazonBestSellersExplorer.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AmazonController : ControllerBase
    {
        private readonly IAmazonApiService _amazonApiService;
        private readonly IFavoriteProductService _favoriteService;

        public AmazonController(IAmazonApiService amazonApiService, IFavoriteProductService favoriteService)
        {
            _amazonApiService = amazonApiService;
            _favoriteService = favoriteService;
        }

        [AllowAnonymous]
        [HttpGet("best-sellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            var result = await _amazonApiService.GetBestSellersAsync();
            return Ok(result);
        }

        [HttpGet("favorites")]
        public async Task<IActionResult> GetFavorites()
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var favorites = await _favoriteService.GetFavoriteAsinsAsync(userId.Value);
            return Ok(favorites);
        }

        [HttpGet("favorites/details")]
        public async Task<IActionResult> GetFavoriteDetails()
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var favorites = await _favoriteService.GetFavoriteDetailsAsync(userId.Value);

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

        [HttpPost("favorites")]
        public async Task<IActionResult> AddFavorite([FromBody] ProductDto dto)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            if (string.IsNullOrWhiteSpace(dto.Asin) || string.IsNullOrWhiteSpace(dto.Title))
                return BadRequest("ASIN i tytuł są wymagane.");

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

            var result = await _favoriteService.AddFavoriteAsync(favorite);
            if (!result.IsSuccess)
                return Conflict(result.ErrorMessage);

            return Ok(new { asin = result.Value!.Asin });
        }

        [HttpDelete("favorites/{asin}")]
        public async Task<IActionResult> RemoveFavorite(string asin)
        {
            var userId = GetCurrentUserId();
            if (userId is null)
                return Unauthorized();

            var result = await _favoriteService.RemoveFavoriteAsync(userId.Value, asin);
            if (!result.IsSuccess)
                return NotFound(result.ErrorMessage);

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub) ?? User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                return userId;

            return null;
        }
    }
}
