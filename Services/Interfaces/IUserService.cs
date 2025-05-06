using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Services.Enums;

namespace TestSignalR.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> FindByLoginAsync(string login);
        public Task<User?> FindByIdAsync(int id);
        public Task<UserViewData?> GetViewDataAsync(int userId);
        public Task ClearNotifyContact(int senderId, int recipientId);
        public Task SetStatusAsync(int userId, UserStatus status);
        public Task<List<string>> GetUserContactIdsAsync(int userId);
    }
}
