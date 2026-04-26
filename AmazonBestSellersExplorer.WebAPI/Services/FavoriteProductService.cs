using System.Collections.Generic;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Repositories;
using AmazonBestSellersExplorer.WebAPI.Services.Core;

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

        public async Task<ServiceResult<FavoriteProduct>> AddFavoriteAsync(FavoriteProduct favoriteProduct)
        {
            var exists = await _repository.ExistsAsync(favoriteProduct.UserId, favoriteProduct.Asin);

            if (exists)
                return ServiceResult<FavoriteProduct>.Failure("Produkt już znajduje się w ulubionych.");

            await _repository.AddAsync(favoriteProduct);

            return ServiceResult<FavoriteProduct>.Success(favoriteProduct);
        }

        public async Task<ServiceResult<bool>> RemoveFavoriteAsync(int userId, string asin)
        {
            var favorite = await _repository.GetByAsinAsync(userId, asin);

            if (favorite == null)
                return ServiceResult<bool>.Failure("Nie znaleziono ulubionego produktu.");

            await _repository.RemoveAsync(favorite);
            return ServiceResult<bool>.Success(true);
        }
    }
}
