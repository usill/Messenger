using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using System.Text;

namespace TestSignalR.Actors
{
    public class Hasher : IHasher
    {
        private readonly IConfiguration _configuration;
        public Hasher(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetHash(string value)
        {
            string salt = _configuration.GetSection("PasswordSalt").Get<string>()!;
            byte[] passwordHashBytes = HMACSHA256.HashData(
                Encoding.UTF8.GetBytes(salt),
                Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(passwordHashBytes);
        }
    }
}
