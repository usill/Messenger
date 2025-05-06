using TestSignalR.Models.Enums;

namespace TestSignalR.Actors
{
    public interface ICookie
    {
        public CookieOptions GetOptions(TokenType tokenType);
    }
}
