﻿using TestSignalR.Models;
using TestSignalR.Models.Enums;

namespace TestSignalR.Services.Interfaces
{
    public interface IAuthService
    {
        public string GenerateAccessToken(string login, string id);
        public string GenerateRefreshToken();
        public Task<User?> ValidateRefreshTokenAsync(string token);
        public Task SetRefreshTokenAsync(string token, int userId);
        public CookieOptions GetCookieOptions(TokenType tokenType);
    }
}
