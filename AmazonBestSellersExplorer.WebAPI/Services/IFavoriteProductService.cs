using System.Collections.Generic;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;

namespace AmazonBestSellersExplorer.WebAPI.Services
{
    public interface IFavoriteProductService
    {
        Task<IEnumerable<string>> GetFavoriteAsinsAsync(int userId);
        Task<IEnumerable<FavoriteProduct>> GetFavoriteDetailsAsync(int userId);
        Task<FavoriteProduct> AddFavoriteAsync(FavoriteProduct favoriteProduct);
        Task<bool> RemoveFavoriteAsync(int userId, string asin);
    }
}
