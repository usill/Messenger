namespace TestSignalR.Services.Interfaces
{
    public interface IAuthService
    {
        public string GenerateJwtToken(string username, string id);
        public CookieOptions GetCookie(string token);
        public string GetPasswordHash(string password);
        public string GetRandomAvatar();
    }
}
