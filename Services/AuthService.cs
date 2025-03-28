using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TestSignalR.Models.Settings;
using TestSignalR.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;

namespace TestSignalR.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly int _tokenLifetime;
        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _tokenLifetime = configuration.GetSection("Auth:TokenLifetimeHourses").Get<int>();
        }
        public string GenerateJwtToken(string username, string id)
        {
            JwtSetting jwtSettings = _configuration.GetSection("Auth:Jwt").Get<JwtSetting>()!;

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, id)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_tokenLifetime),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key)), SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public CookieOptions GetCookie(string token)
        {
            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddHours(_tokenLifetime)
            };
        }
        public string GetPasswordHash(string password)
        {
            string salt = _configuration.GetSection("PasswordSalt").Get<string>()!;
            byte[] passwordHashBytes = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(salt),
                Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(passwordHashBytes);
        }
        public string GetRandomAvatar()
        {
            string avatarsDir = @_configuration.GetSection("Directories:Avatar").Get<string>()!;
            string[] files = Directory.GetFiles(avatarsDir);
            Random rand = new Random();
            string file = files[rand.Next(files.Length)];
            return Path.GetFileName(file);
        }
    }
}
