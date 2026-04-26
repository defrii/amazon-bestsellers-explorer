using System.ComponentModel.DataAnnotations;

namespace AmazonBestSellersExplorer.WebAPI.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Login { get; set; } = null!;
        
        [Required]
        public string PasswordHash { get; set; } = null!;
        public List<FavoriteProduct> FavoriteProducts { get; set; } = new();
    }
}


