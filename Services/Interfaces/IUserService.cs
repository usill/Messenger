using TestSignalR.Models;
using TestSignalR.Models.DTO;

namespace TestSignalR.Services.Interfaces
{
    public interface IUserService
    {
        public Task<User?> FindByNameAsync(string username);
        public Task<User?> FindByIdAsync(int id);
        public Task<UserViewData?> GetViewDataAsync(int userId);
        public Task ClearNotifyContact(int senderId, int recipientId);
    }
}
