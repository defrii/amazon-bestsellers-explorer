using System;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Helpers;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Repositories;
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

        public async Task<User> RegisterUserAsync(User user, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(user.Login) || user.Login.Length < 5 || user.Login.Length > 50)
                throw new ArgumentException("Login must be between 5 and 50 characters.");

            if (!ValidationHelper.IsValidLoginCharacters(user.Login))
                throw new ArgumentException("Login can only contain English letters, numbers, and special characters.");

            if (string.IsNullOrWhiteSpace(rawPassword) || rawPassword.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters.");

            if (!ValidationHelper.IsValidPasswordComplexity(rawPassword))
                throw new ArgumentException("Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character.");

            var exists = await _userRepository.ExistsByLoginAsync(user.Login);
            if (exists)
                throw new InvalidOperationException("Login already exists.");

            user.PasswordHash = _passwordHasher.HashPassword(user, rawPassword);

            return await _userRepository.AddAsync(user);
        }

        public async Task<User?> AuthenticateUserAsync(string login, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(rawPassword))
                throw new ArgumentException("Login and password required.");

            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null)
                return null;

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, rawPassword);
            if (verify == PasswordVerificationResult.Failed)
                return null;

            return user;
        }
    }
}
