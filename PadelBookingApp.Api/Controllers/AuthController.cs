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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDto userDto)
        {
            // Check if the user already exists
            var existingUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            if (existingUser != null)
            {
                return BadRequest("User already exists");
            }

            // Hash the password before storing it.
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.Password);

            // Create a new user with the hashed password.
            var user = new User
            {
                Email = userDto.Email,
                Password = hashedPassword,
                Role = "Customer"
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserDto userDto)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email);
            // Verify the entered password against the stored hash.
            if (user == null || !BCrypt.Net.BCrypt.Verify(userDto.Password, user.Password))
            {
                return Unauthorized("Invalid credentials");
            }

            var accessToken = GenerateJwtToken(user.Email);
            var refreshToken = Guid.NewGuid().ToString(); // In production, implement proper refresh token handling.

            return Ok(new { AccessToken = accessToken, RefreshToken = refreshToken });
        }

        private string GenerateJwtToken(string email)
        {
            // Use a consistent 'now'
            var now = DateTime.UtcNow;
            // Ensure AccessTokenExpirationMinutes is greater than zero.
            var expirationMinutes = _jwtSettings.AccessTokenExpirationMinutes;
            if(expirationMinutes <= 0)
            {
                // Fallback value; you can also throw an error if this isn't acceptable.
                expirationMinutes = 15;
            }
            
            var expires = now.AddMinutes(expirationMinutes);
            
            var keyValue = _jwtSettings.Key;
            if (string.IsNullOrEmpty(keyValue))
            {
                // Fallback: get the key directly from environment.
                keyValue = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
            }
            if (string.IsNullOrWhiteSpace(keyValue))
            {
                throw new InvalidOperationException("JWT key is not configured.");
            }
            var key = Encoding.ASCII.GetBytes(keyValue);

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, email)
                }),
                // Set IssuedAt and NotBefore to 'now' so that Expires is guaranteed to be later.
                IssuedAt = now,
                NotBefore = now,
                Expires = expires,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
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