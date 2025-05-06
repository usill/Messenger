using Microsoft.Extensions.Configuration;

namespace TestSignalR.Actors
{
    public class Avatar : IAvatar
    {
        private readonly IConfiguration _configuration;
        public Avatar(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public string GetRandom()
        {
            string avatarsDir = @_configuration.GetSection("Directories:Avatar").Get<string>()!;
            string[] files = Directory.GetFiles(avatarsDir);
            Random rand = new Random();
            string file = files[rand.Next(files.Length)];
            return Path.GetFileName(file);
        }
    }
}
