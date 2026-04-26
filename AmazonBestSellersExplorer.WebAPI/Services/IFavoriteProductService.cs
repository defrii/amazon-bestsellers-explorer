using System.Collections.Generic;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Services.Core;

namespace AmazonBestSellersExplorer.WebAPI.Services
{
    public interface IFavoriteProductService
    {
        Task<IEnumerable<string>> GetFavoriteAsinsAsync(int userId);
        Task<IEnumerable<FavoriteProduct>> GetFavoriteDetailsAsync(int userId);
        Task<ServiceResult<FavoriteProduct>> AddFavoriteAsync(FavoriteProduct favoriteProduct);
        Task<ServiceResult<bool>> RemoveFavoriteAsync(int userId, string asin);
    }
}
