namespace AmazonBestSellersExplorer.WebAPI.Models
{
    public class FavoriteProduct
    {
        public int FavoriteProductId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Price { get; set; }

        // Amazon Standard Identification Number
        public string Asin { get; set; } = string.Empty;

        public string? Url { get; set; }

        public string? Photo { get; set; }

        public double? StarRating { get; set; }

        // Foreign key
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}

