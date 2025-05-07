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
using TestSignalR.Actors;

namespace TestSignalR.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly int _accessTokenLifetimeMinutes;
        private readonly int _refreshTokenLifetimeDays;
        private readonly IHasher _hasher;
        private readonly AppDbContext _dbContext;
        private readonly ICookie _cookie;

        public AuthService(IConfiguration configuration, IHasher hasher, AppDbContext dbContext, ICookie cookie)
        {
            _configuration = configuration;
            _hasher = hasher;
            _dbContext = dbContext;
            _cookie = cookie;
            _accessTokenLifetimeMinutes = configuration.GetSection("Auth:AccessTokenLifetimeMinutes").Get<int>();
            _refreshTokenLifetimeDays = configuration.GetSection("Auth:RefreshTokenLifetimeDays").Get<int>();
        }
        public string GenerateAccessToken(string login, string id)
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

            string hash = _hasher.GetHash(token);

            RefreshToken? storedToken = await _dbContext.RefreshTokens.Include(t => t.User).Where(t => t.HashValue == hash && t.ExpiresAt > DateTime.UtcNow).FirstOrDefaultAsync();
            List<RefreshToken> tokens = await _dbContext.RefreshTokens.ToListAsync();

            if (storedToken == null)
            {
                return null;
            }

            return storedToken.User;
        }
        public async Task SetRefreshTokenAsync(string token, int userId)
        {
            string hash = _hasher.GetHash(token);

            RefreshToken? storedToken = await _dbContext.RefreshTokens.Where(t => t.UserId == userId).FirstOrDefaultAsync();

            if (storedToken != null)
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
        public CookieOptions GetCookieOptions(TokenType tokenType) => _cookie.GetOptions(tokenType);
    }
}
