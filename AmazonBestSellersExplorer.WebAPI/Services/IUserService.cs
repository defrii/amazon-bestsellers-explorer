using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;

namespace AmazonBestSellersExplorer.WebAPI.Services
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(User user, string rawPassword);
        Task<User?> AuthenticateUserAsync(string login, string rawPassword);
    }
}
