using TestSignalR.Models;

namespace TestSignalR.Services.Interfaces
{
    public interface IUserService
    {
        public Task<List<User>> GetContactsAsync(int userId);
        public Task<string> GetAvatar(int userId);
        public Task<User?> FindByNameAsync(string username);
        public Task<User?> FindByIdAsync(int id);
        public Task AddContactAsync(User u1, User u2);
    }
}
