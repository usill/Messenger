using TestSignalR.Models;

namespace TestSignalR.Services.Interfaces
{
    public interface IUserService
    {
        public Task<List<User>> GetContactsAsync(int userId);
        public Task<string> GetAvatar(int userId);
    }
}
