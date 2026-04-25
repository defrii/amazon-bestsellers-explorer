using Microsoft.AspNetCore.Mvc;
using AmazonBestSellersExplorer.WebAPI.Data;
using AmazonBestSellersExplorer.WebAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace AmazonBestSellersExplorer.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext db, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
        {
            _db = db;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
        }

        public class RegisterDto
        {
            public string Login { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        public class LoginDto
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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Login) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Login and password required.");

            var user = _db.Users.SingleOrDefault(u => u.Login == dto.Login);
            if (user == null)
                return Unauthorized("Invalid credentials");

            var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (verify == PasswordVerificationResult.Failed)
                return Unauthorized("Invalid credentials");

            // Create JWT
            var jwtKey = _configuration["Jwt:Key"] ?? throw new InvalidOperationException("Jwt:Key is not configured");
            var jwtIssuer = _configuration["Jwt:Issuer"];
            var jwtAudience = _configuration["Jwt:Audience"];
            var expiresMinutes = int.Parse(_configuration["Jwt:ExpiresMinutes"] ?? "60");

            
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Login),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString, expires = token.ValidTo, userId = user.UserId, login = user.Login });
        }
    }
}

