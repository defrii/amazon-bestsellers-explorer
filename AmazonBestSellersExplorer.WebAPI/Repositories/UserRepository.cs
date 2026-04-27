using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Data;
using Microsoft.EntityFrameworkCore;

namespace AmazonBestSellersExplorer.WebAPI.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<bool> ExistsByLoginAsync(string login)
        {
            return await _db.Users.AnyAsync(u => u.Login == login);
        }

        public async Task<User?> GetByLoginAsync(string login)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Login == login);
        }

        public async Task<User> AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}
