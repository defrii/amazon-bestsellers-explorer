namespace AmazonBestSellersExplorer.WebAPI.Models
{
    public class FavoriteProduct
    {
        public int FavoriteProductId { get; set; }

        public string Title { get; set; } = null!;

        public decimal Price { get; set; }

        public string? Url { get; set; }

        public string? Photo { get; set; }

        public double? StarRating { get; set; }

        // Foreign key
        public int UserId { get; set; }

        public User? User { get; set; }
    }
}

