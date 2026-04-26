using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Helpers;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Repositories;
using AmazonBestSellersExplorer.WebAPI.Services.Core;
using Microsoft.AspNetCore.Identity;

namespace AmazonBestSellersExplorer.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher<User> passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<ServiceResult<User>> RegisterUserAsync(User user, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(user.Login) || user.Login.Length < 5 || user.Login.Length > 50)
                return ServiceResult<User>.Failure("Login must be between 5 and 50 characters.");

            if (!ValidationHelper.IsValidLoginCharacters(user.Login))
                return ServiceResult<User>.Failure("Login can only contain letters and numbers.");

            if (string.IsNullOrWhiteSpace(rawPassword) || rawPassword.Length < 8)
                return ServiceResult<User>.Failure("Password must be at least 8 characters.");

            if (!ValidationHelper.IsValidPasswordComplexity(rawPassword))
                return ServiceResult<User>.Failure("Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character.");

            var exists = await _userRepository.ExistsByLoginAsync(user.Login);
            if (exists)
                return ServiceResult<User>.Failure("Login already exists.");

            user.PasswordHash = _passwordHasher.HashPassword(user, rawPassword);

            var created = await _userRepository.AddAsync(user);
            return ServiceResult<User>.Success(created);
        }

        public async Task<ServiceResult<User>> AuthenticateUserAsync(string login, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(rawPassword))
                return ServiceResult<User>.Failure("Login and password required.");

            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null)
                return ServiceResult<User>.Failure("Invalid credentials.");

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, rawPassword);
            if (verify == PasswordVerificationResult.Failed)
                return ServiceResult<User>.Failure("Invalid credentials.");

            return ServiceResult<User>.Success(user);
        }
    }
}
