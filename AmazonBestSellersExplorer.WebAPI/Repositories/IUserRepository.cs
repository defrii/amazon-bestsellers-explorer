using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Data;

namespace AmazonBestSellersExplorer.WebAPI.Repositories
{
    public interface IUserRepository
    {
        Task<bool> ExistsByLoginAsync(string login);
        Task<User?> GetByLoginAsync(string login);
        Task<User> AddAsync(User user);
    }
}
