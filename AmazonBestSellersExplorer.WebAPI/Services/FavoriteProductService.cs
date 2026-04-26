using System.Collections.Generic;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Repositories;

namespace AmazonBestSellersExplorer.WebAPI.Services
{
    public class FavoriteProductService : IFavoriteProductService
    {
        private readonly IFavoriteProductRepository _repository;

        public FavoriteProductService(IFavoriteProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<string>> GetFavoriteAsinsAsync(int userId)
        {
            return await _repository.GetFavoriteAsinsAsync(userId);
        }

        public async Task<IEnumerable<FavoriteProduct>> GetFavoriteDetailsAsync(int userId)
        {
            return await _repository.GetFavoritesAsync(userId);
        }

        public async Task<FavoriteProduct> AddFavoriteAsync(FavoriteProduct favoriteProduct)
        {
            var exists = await _repository.ExistsAsync(favoriteProduct.UserId, favoriteProduct.Asin);

            if (exists)
                throw new System.InvalidOperationException("Product is already in favorites.");

            await _repository.AddAsync(favoriteProduct);

            return favoriteProduct;
        }

        public async Task<bool> RemoveFavoriteAsync(int userId, string asin)
        {
            var favorite = await _repository.GetByAsinAsync(userId, asin);

            if (favorite == null)
                return false;

            await _repository.RemoveAsync(favorite);
            return true;
        }
    }
}
