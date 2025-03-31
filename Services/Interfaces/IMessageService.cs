using TestSignalR.Models;
using TestSignalR.Models.DTO;

namespace TestSignalR.Services.Interfaces
{
    public interface IMessageService
    {
        public Task<List<Message>> GetMessagesByUserAsync(int recipientId, int senderId, int limit = 25, string order = "ASC");
        public Task<SendMessageResult?> SendMessage(string receiverId, string senderId, string message);
    }
}
