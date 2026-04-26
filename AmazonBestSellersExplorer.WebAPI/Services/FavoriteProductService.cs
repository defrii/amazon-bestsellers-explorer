using System.Collections.Generic;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Repositories;

namespace AmazonBestSellersExplorer.WebAPI.Services
{
    public class FavoriteProductService : IFavoriteProductService
    {
        private readonly IFavoriteProductRepository _favoriteProductRepository;

        public FavoriteProductService(IFavoriteProductRepository favoriteProductRepository)
        {
            _favoriteProductRepository = favoriteProductRepository;
        }

        public async Task<IEnumerable<string>> GetFavoriteAsinsAsync(int userId)
        {
            return await _favoriteProductRepository.GetFavoriteAsinsAsync(userId);
        }

        public async Task<IEnumerable<FavoriteProduct>> GetFavoriteDetailsAsync(int userId)
        {
            return await _favoriteProductRepository.GetFavoritesAsync(userId);
        }

        public async Task<FavoriteProduct> AddFavoriteAsync(FavoriteProduct favoriteProduct)
        {
            var exists = await _favoriteProductRepository.ExistsAsync(favoriteProduct.UserId, favoriteProduct.Asin);

            if (exists)
                throw new System.InvalidOperationException("Product is already in favorites.");

            await _favoriteProductRepository.AddAsync(favoriteProduct);

            return favoriteProduct;
        }

        public async Task<bool> RemoveFavoriteAsync(int userId, string asin)
        {
            var favorite = await _favoriteProductRepository.GetByAsinAsync(userId, asin);

            if (favorite == null)
                return false;

            await _favoriteProductRepository.RemoveAsync(favorite);
            return true;
        }
    }
}
