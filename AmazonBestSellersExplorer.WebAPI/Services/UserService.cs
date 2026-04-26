using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Models;
using AmazonBestSellersExplorer.WebAPI.Repositories;
using Microsoft.AspNetCore.Identity;

namespace AmazonBestSellersExplorer.WebAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUserRepository repository, IPasswordHasher<User> passwordHasher)
        {
            _repository = repository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> RegisterUserAsync(User user, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(user.Login) || user.Login.Length < 5 || user.Login.Length > 50)
                throw new ArgumentException("Login must be between 5 and 50 characters.");

            if (!Regex.IsMatch(user.Login, @"^[a-zA-Z0-9!@#\$%\^&\*\(\)_\+\-\=\[\]\{\}\|;:'"",\.<>\/\?\`~]+$"))
                throw new ArgumentException("Login can only contain English letters, numbers, and special characters.");

            if (string.IsNullOrWhiteSpace(rawPassword) || rawPassword.Length < 8)
                throw new ArgumentException("Password must be at least 8 characters.");

            if (!Regex.IsMatch(rawPassword, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,}$"))
                throw new ArgumentException("Password must contain at least one lowercase letter, one uppercase letter, one number, and one special character.");

            var exists = await _repository.ExistsByLoginAsync(user.Login);
            if (exists)
                throw new InvalidOperationException("Login already exists.");

            user.PasswordHash = _passwordHasher.HashPassword(user, rawPassword);

            return await _repository.AddAsync(user);
        }

        public async Task<User?> AuthenticateUserAsync(string login, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(rawPassword))
                throw new ArgumentException("Login and password required.");

            var user = await _repository.GetByLoginAsync(login);
            if (user == null)
                return null;

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, rawPassword);
            if (verify == PasswordVerificationResult.Failed)
                return null;

            return user;
        }
    }
}
