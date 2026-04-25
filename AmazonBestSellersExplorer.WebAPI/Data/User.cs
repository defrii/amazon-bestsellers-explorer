using System.ComponentModel.DataAnnotations;

namespace AmazonBestSellersExplorer.WebAPI.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Login { get; set; } = null!;

        // PasswordHash stores the full hashed password produced by a password hasher (contains salt internally for ASP.NET Identity)
        [Required]
        public string PasswordHash { get; set; } = null!;

        public List<FavoriteProduct> FavoriteProducts { get; set; } = new();
    }
}


