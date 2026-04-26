using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AmazonBestSellersExplorer.WebAPI.Data;
using AmazonBestSellersExplorer.WebAPI.Models;
using System.Security.Claims;
using System.Globalization;

namespace AmazonBestSellersExplorer.WebAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FavoritesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public FavoritesController(ApplicationDbContext db)
        {
            _db = db;
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
                var favorites = await _db.FavoriteProducts
                    .Where(f => f.UserId == userId)
                    .Select(f => f.Asin)
                    .ToListAsync();

                return Ok(favorites);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }

        public class FavoriteProductDto
        {
            public string Title { get; set; } = null!;
            public string? Price { get; set; }
            public string Asin { get; set; } = null!;
            public string? Url { get; set; }
            public string? Photo { get; set; }
            public string? StarRating { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteProductDto dto)
        {
            try
            {
                var userId = GetCurrentUserId();

                if (string.IsNullOrWhiteSpace(dto.Asin) || string.IsNullOrWhiteSpace(dto.Title))
                    return BadRequest("ASIN and Title are required.");

                var exists = await _db.FavoriteProducts
                    .AnyAsync(f => f.UserId == userId && f.Asin == dto.Asin);

                if (exists)
                    return Conflict("Product is already in favorites.");

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
                    var ratingMatch = System.Text.RegularExpressions.Regex.Match(dto.StarRating, @"([0-9.]+)");
                    if (ratingMatch.Success)
                    {
                        double.TryParse(ratingMatch.Groups[1].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out parsedRating);
                    }
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

                _db.FavoriteProducts.Add(favorite);
                await _db.SaveChangesAsync();

                return Ok(new { asin = favorite.Asin });
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

                var favorite = await _db.FavoriteProducts
                    .FirstOrDefaultAsync(f => f.UserId == userId && f.Asin == asin);

                if (favorite == null)
                    return NotFound();

                _db.FavoriteProducts.Remove(favorite);
                await _db.SaveChangesAsync();

                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized();
            }
        }
    }
}
