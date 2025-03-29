using TestSignalR.Models;

namespace TestSignalR.Services.Interfaces
{
    public interface IMessageService
    {
        public Task<List<Message>> GetMessagesByUserAsync(int recipientId, int senderId);
    }
}
