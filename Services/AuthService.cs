using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TestSignalR.Models.Settings;
using TestSignalR.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using TestSignalR.Models;
using Microsoft.EntityFrameworkCore;
using TestSignalR.Models.Enums;

namespace TestSignalR.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly int _accessTokenLifetimeMinutes;
        private readonly int _refreshTokenLifetimeDays;
        private readonly AppDbContext _dbContext;
        public AuthService(IConfiguration configuration, AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _configuration = configuration;
            _accessTokenLifetimeMinutes = configuration.GetSection("Auth:AccessTokenLifetimeMinutes").Get<int>();
            _refreshTokenLifetimeDays = configuration.GetSection("Auth:RefreshTokenLifetimeDays").Get<int>();
        }
        public string GenerateJwtToken(string login, string id)
        {
            JwtSetting jwtSettings = _configuration.GetSection("Auth:Jwt").Get<JwtSetting>()!;

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Name, login),
                new Claim(ClaimTypes.NameIdentifier, id)
            };

            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Key));

            var token = new JwtSecurityToken(
                issuer: jwtSettings.Issuer,
                audience: jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(_accessTokenLifetimeMinutes),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            byte[] randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
        public async Task<User?> ValidateRefreshTokenAsync(string token)
        {
            if (token.IsNullOrEmpty())
                return null;

            string hash = GetPasswordHash(token);

            RefreshToken? storedToken = await _dbContext.RefreshTokens.Include(t => t.User).Where(t => t.HashValue == hash && t.ExpiresAt > DateTime.UtcNow).FirstOrDefaultAsync();
            List<RefreshToken> tokens = await _dbContext.RefreshTokens.ToListAsync();

            if(storedToken == null)
            {
                return null;
            }

            return storedToken.User;
        }
        public async Task SetRefreshTokenAsync(string token, int userId)
        {
            string hash = GetPasswordHash(token);

            RefreshToken? storedToken = await _dbContext.RefreshTokens.Where(t => t.UserId == userId).FirstOrDefaultAsync();

            if(storedToken != null)
            {
                storedToken.HashValue = hash;
                storedToken.ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenLifetimeDays);
            } 
            else
            {
                RefreshToken newToken = new RefreshToken
                {
                    HashValue = hash,
                    ExpiresAt = DateTime.UtcNow.AddDays(_refreshTokenLifetimeDays),
                    UserId = userId
                };
                _dbContext.RefreshTokens.Add(newToken);
            }

            await _dbContext.SaveChangesAsync();
        }
        public CookieOptions GetCookieOptions(TokenType tokenType)
        {
            DateTime expires =
                tokenType == TokenType.Access ?
                DateTime.UtcNow.AddMinutes(_accessTokenLifetimeMinutes) :
                DateTime.UtcNow.AddDays(_refreshTokenLifetimeDays);

            return new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = expires
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
