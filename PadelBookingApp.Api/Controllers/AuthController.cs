using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PadelBookingApp.Api.Models;
using PadelBookingApp.Domain.Entities;
using PadelBookingApp.Infrastructure;
using BCrypt.Net;

namespace PadelBookingApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly JwtSettings _jwtSettings;

        public AuthController(ApplicationDbContext dbContext, IOptions<JwtSettings> jwtSettings)
        {
            _dbContext = dbContext;
            _jwtSettings = jwtSettings.Value;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            var accessToken = GenerateJwtToken(user);
            var refreshToken = Guid.NewGuid().ToString(); // For production: implement proper refresh token handling.
            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        private string GenerateJwtToken(User user)
        {
            var now = DateTime.UtcNow;
            var expirationMinutes = _jwtSettings.AccessTokenExpirationMinutes > 0 ? _jwtSettings.AccessTokenExpirationMinutes : 15;
            var expires = now.AddMinutes(expirationMinutes);

            var keyValue = !string.IsNullOrEmpty(_jwtSettings.Key) ? _jwtSettings.Key : Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            if (string.IsNullOrWhiteSpace(keyValue))
                throw new InvalidOperationException("JWT key is not configured.");

            var keyBytes = Encoding.ASCII.GetBytes(keyValue);
            var symmetricKey = new SymmetricSecurityKey(keyBytes)
            {
                KeyId = "default_key"
            };

            // Include role claim along with the user's email.
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // e.g. "Customer" or "Admin"
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                IssuedAt = now,
                NotBefore = now,
                Expires = expires,
                Issuer = string.IsNullOrEmpty(_jwtSettings.Issuer) ? "PadelBookingApp" : _jwtSettings.Issuer,
                Audience = string.IsNullOrEmpty(_jwtSettings.Audience) ? "PadelBookingAppUsers" : _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class UserDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}