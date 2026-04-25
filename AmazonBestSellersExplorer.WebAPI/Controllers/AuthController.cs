using Microsoft.AspNetCore.Mvc;
using AmazonBestSellersExplorer.WebAPI.Data;
using AmazonBestSellersExplorer.WebAPI.Models;
using Microsoft.AspNetCore.Identity;

namespace AmazonBestSellersExplorer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AuthController(ApplicationDbContext db, IPasswordHasher<User> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        public class RegisterDto
        {
            public string Login { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Login) || dto.Login.Length < 5 || dto.Login.Length > 50)
                return BadRequest("Login must be between 5 and 50 characters.");

            if (string.IsNullOrWhiteSpace(dto.Password) || dto.Password.Length < 6)
                return BadRequest("Password must be at least 6 characters.");

            if (_db.Users.Any(u => u.Login == dto.Login))
                return Conflict("Login already exists.");

            var user = new User
            {
                Login = dto.Login
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return CreatedAtAction(null, new { id = user.UserId }, new { userId = user.UserId, login = user.Login });
        }
    }
}

