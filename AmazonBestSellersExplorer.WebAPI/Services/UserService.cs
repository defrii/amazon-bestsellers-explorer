using System.Threading.Tasks;
using AmazonBestSellersExplorer.WebAPI.Data;
using AmazonBestSellersExplorer.WebAPI.Helpers;
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
                return ServiceResult<User>.Failure("Login musi mieć pomiędzy 5 i 50 znaków.");

            if (!ValidationHelper.IsValidLoginCharacters(user.Login))
                return ServiceResult<User>.Failure("Login może zawierać wyłącznie litery i cyfry.");

            if (string.IsNullOrWhiteSpace(rawPassword) || rawPassword.Length < 8)
                return ServiceResult<User>.Failure("Hasło musi mieć długość przynajmniej 8 znaków.");

            if (!ValidationHelper.IsValidPasswordComplexity(rawPassword))
                return ServiceResult<User>.Failure("Hasło musi zawierać minimum 1: mała litera, wielka litera, cyfra, znak specjalny.");

            var exists = await _userRepository.ExistsByLoginAsync(user.Login);
            if (exists)
                return ServiceResult<User>.Failure("Login już istnieje.");

            user.PasswordHash = _passwordHasher.HashPassword(user, rawPassword);

            var created = await _userRepository.AddAsync(user);
            return ServiceResult<User>.Success(created);
        }

        public async Task<ServiceResult<User>> AuthenticateUserAsync(string login, string rawPassword)
        {
            if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(rawPassword))
                return ServiceResult<User>.Failure("Login oraz hasło są wymagane.");

            var user = await _userRepository.GetByLoginAsync(login);
            if (user == null)
                return ServiceResult<User>.Failure("Nieprawidłowy login lub hasło.");// wysyłamy ogólny komunikat o błędzie zamiast wskazywać,
                                                                                     // czy problem dotyczy loginu lub hasła

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, rawPassword);
            if (verify == PasswordVerificationResult.Failed)
                return ServiceResult<User>.Failure("Nieprawidłowy login lub hasło."); 

            return ServiceResult<User>.Success(user);
        }
    }
}
