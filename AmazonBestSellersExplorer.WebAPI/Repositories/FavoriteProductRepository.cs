using AmazonBestSellersExplorer.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AmazonBestSellersExplorer.WebAPI.Repositories
{
    public class FavoriteProductRepository : IFavoriteProductRepository
    {
        private readonly ApplicationDbContext _db;

        public FavoriteProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IEnumerable<string>> GetFavoriteAsinsAsync(int userId)
        {
            return await _db.FavoriteProducts
                .Where(f => f.UserId == userId)
                .Select(f => f.Asin)
                .ToListAsync();
        }

        public async Task<IEnumerable<FavoriteProduct>> GetFavoritesAsync(int userId)
        {
            return await _db.FavoriteProducts
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int userId, string asin)
        {
            return await _db.FavoriteProducts
                .AnyAsync(f => f.UserId == userId && f.Asin == asin);
        }

        public async Task AddAsync(FavoriteProduct favoriteProduct)
        {
            _db.FavoriteProducts.Add(favoriteProduct);
            await _db.SaveChangesAsync();
        }

        public async Task<FavoriteProduct?> GetByAsinAsync(int userId, string asin)
        {
            return await _db.FavoriteProducts
                .FirstOrDefaultAsync(f => f.UserId == userId && f.Asin == asin);
        }

        public async Task RemoveAsync(FavoriteProduct favoriteProduct)
        {
            _db.FavoriteProducts.Remove(favoriteProduct);
            await _db.SaveChangesAsync();
        }
    }
}
