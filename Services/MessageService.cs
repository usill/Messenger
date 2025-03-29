using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;
        public MessageService(AppDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }
        public async Task<List<Message>> GetMessagesByUserAsync(int recipientId, int senderId, int limit = 25)
        {
            return await _dbContext.Messages
                .FromSqlInterpolated($"SELECT * FROM Messages WHERE RecipientId = {recipientId} AND SenderId = {senderId} OR SenderId = {recipientId} AND RecipientId = {senderId} ORDER BY SendedAt ASC LIMIT {limit}")
                .ToListAsync();
        }
        public async Task<SendMessageResult?> SendMessage(string receiverId, string senderId, string message)
        {
            SendMessageResult result = new SendMessageResult();
            result.IsNewContact = false;
            int senderNumericId = Convert.ToInt32(senderId);
            int receiverNumericId;
            User? sender = _dbContext.Users.Where(u => u.Id == senderNumericId).FirstOrDefault();
            
            if (!Int32.TryParse(receiverId, out receiverNumericId))
            {
                return null;
            }

            if (sender == null)
            {
                return null;
            }

            List<Message> lastMsg = await GetMessagesByUserAsync(receiverNumericId, senderNumericId, 1);
            if (lastMsg.Count == 0)
            {
                User? receiver = await _userService.FindByIdAsync(receiverNumericId);

                if(receiver != null)
                {
                    await _userService.AddContactAsync(receiver, sender);
                    result.IsNewContact = true;
                    result.Sender = sender;
                    result.Receiver = receiver;
                }
            }

            Message msg = new Message
            {
                SenderId = senderNumericId,
                RecipientId = receiverNumericId,
                SendedAt = DateTime.UtcNow,
                Text = message
            };

            sender.MessagesSended.Add(msg);
            await _dbContext.SaveChangesAsync();

            return result;
        }
    }
}
