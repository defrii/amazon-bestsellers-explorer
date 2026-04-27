namespace AmazonBestSellersExplorer.WebAPI.Data
{
    public class FavoriteProduct
    {
        public int FavoriteProductId { get; set; }
        public string Title { get; set; } = null!;
        public string? Price { get; set; }
        public string Asin { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Photo { get; set; }
        public double? StarRating { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}

