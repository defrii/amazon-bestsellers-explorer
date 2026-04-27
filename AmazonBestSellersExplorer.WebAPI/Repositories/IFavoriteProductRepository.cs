using AmazonBestSellersExplorer.WebAPI.Data;

namespace AmazonBestSellersExplorer.WebAPI.Repositories
{
    public interface IFavoriteProductRepository
    {
        Task<IEnumerable<string>> GetFavoriteAsinsAsync(int userId);
        Task<IEnumerable<FavoriteProduct>> GetFavoritesAsync(int userId);
        Task<bool> ExistsAsync(int userId, string asin);
        Task AddAsync(FavoriteProduct favoriteProduct);
        Task<FavoriteProduct?> GetByAsinAsync(int userId, string asin);
        Task RemoveAsync(FavoriteProduct favoriteProduct);
    }
}
