using TestSignalR.Models;
using TestSignalR.Models.DTO.response;

namespace TestSignalR.Services.Interfaces
{
    public interface IMessageService
    {
        public Task<List<Message>> GetMessagesByUserAsync(int recipientId, int senderId, int index = 0, int limit = 50, string order = "DESC");
        public Task<SendMessageResponse?> SendMessage(string receiverId, string senderId, string message);
    }
}
