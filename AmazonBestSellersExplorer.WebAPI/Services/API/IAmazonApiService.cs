using System.Collections.Generic;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Dto;

namespace AmazonBestSellersExplorer.WebAPI.Services.API
{
    public interface IAmazonApiService
    {
        Task<IEnumerable<ProductDto>> GetBestSellersAsync();
    }
}
