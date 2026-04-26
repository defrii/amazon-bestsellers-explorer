using System.Collections.Generic;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;

namespace AmazonBestSellersExplorer.WebAPI.Services.API
{
    public interface IAmazonApiService
    {
        Task<IEnumerable<ProductDto>> GetBestSellersAsync();
    }
}
