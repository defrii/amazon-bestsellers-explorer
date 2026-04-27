using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Data;
using AmazonBestSellersExplorer.WebAPI.Services.Core;

namespace AmazonBestSellersExplorer.WebAPI.Services
{
    public interface IUserService
    {
        Task<ServiceResult<User>> RegisterUserAsync(User user, string rawPassword);
        Task<ServiceResult<User>> AuthenticateUserAsync(string login, string rawPassword);
    }
}
