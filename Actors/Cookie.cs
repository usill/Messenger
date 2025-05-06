using TestSignalR.Models.Enums;

namespace TestSignalR.Actors
{
    public class Cookie : ICookie
    {
        private readonly int _accessTokenLifetimeMinutes;
        private readonly int _refreshTokenLifetimeDays;
        public Cookie(IConfiguration configuration)
        {
            _accessTokenLifetimeMinutes = configuration.GetSection("Auth:AccessTokenLifetimeMinutes").Get<int>();
            _refreshTokenLifetimeDays = configuration.GetSection("Auth:RefreshTokenLifetimeDays").Get<int>();
        }
        public CookieOptions GetOptions(TokenType tokenType)
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
    }
}
