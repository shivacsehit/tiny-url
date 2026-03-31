using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TinyUrl.API.Interfaces;
using TinyUrl.API.Models;

namespace TinyUrl.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _config;

        public AuthService(IConfiguration config)
        {
            _config = config;
        }

        public AuthResponse? LoginWithCredentials(
            string username, string password)
        {
            var adminUsername = _config["AdminCredentials:Username"];
            var adminPassword = _config["AdminCredentials:Password"];

            if (username != adminUsername || password != adminPassword)
                return null;

            var token = GenerateToken(username);
            var expiry = int.Parse(
                _config["JwtSettings:ExpiryMinutes"] ?? "60");

            return new AuthResponse(
                Token: token,
                TokenType: "Bearer",
                ExpiresIn: expiry * 60,
                Username: username
            );
        }

        public AuthResponse? LoginWithApiKey(string apiKey)
        {
            var validApiKey = _config["ApiKey"];
            if (apiKey != validApiKey) return null;

            var token = GenerateToken("api-user");
            var expiry = int.Parse(
                _config["JwtSettings:ExpiryMinutes"] ?? "60");

            return new AuthResponse(
                Token: token,
                TokenType: "Bearer",
                ExpiresIn: expiry * 60,
                Username: "api-user"
            );
        }

        public string GenerateToken(string username)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _config["JwtSettings:SecretKey"]!));

            var credentials = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User"),
                new Claim(JwtRegisteredClaimNames.Jti,
                    Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat,
                    DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                        .ToString())
            };

            var expiry = int.Parse(
                _config["JwtSettings:ExpiryMinutes"] ?? "60");

            var token = new JwtSecurityToken(
                issuer: _config["JwtSettings:Issuer"],
                audience: _config["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiry),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}