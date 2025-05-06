using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestSignalR.Models;
using TestSignalR.Models.DTO.response;
using TestSignalR.Models.Helper;
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
        public async Task<List<Message>> GetMessagesByUserAsync(int recipientId, int senderId, int index = 0, int limit = 50, string order = "DESC")
        {
            int offset = index * limit;

            var messages = await _dbContext.Messages
                    .Where(m =>
                        (m.RecipientId == recipientId && m.SenderId == senderId) ||
                        (m.SenderId == recipientId && m.RecipientId == senderId))
                    .OrderBy(m => order.ToUpper() == "DESC" ? m.SendedAt : m.SendedAt)
                    .Skip(offset)
                    .Take(offset)
                    .ToListAsync();

            return messages;
        }
        public async Task<SendMessageResponse?> SendMessage(string receiverId, string senderId, string message)
        {
            SendMessageResponse result = new SendMessageResponse();
            result.IsNewReceiver = false;
            result.IsNewSender = false;
            int senderNumericId = Convert.ToInt32(senderId);
            int receiverNumericId;

            if (!Int32.TryParse(receiverId, out receiverNumericId))
            {
                return null;
            }

            User? sender = _dbContext.Users.Where(u => u.Id == senderNumericId).FirstOrDefault();
            User? receiver = await _userService.FindByIdAsync(receiverNumericId);

            if (sender == null || receiver == null)
            {
                return null;
            }

            var senderContainsRceiver = await _dbContext.Contacts.Include(c => c.User).Where(c => c.OwnerId == senderNumericId && c.User.Id == receiverNumericId).FirstOrDefaultAsync();
            var receiverContainsSender = await _dbContext.Contacts.Include(c => c.User).Where(c => c.OwnerId == receiverNumericId && c.User.Id == senderNumericId).FirstOrDefaultAsync();

            if (senderContainsRceiver == null)
            {
                sender.Contacts.Add(new Contact
                {
                    User = receiver,
                    HasNewMessage = false
                });
                result.IsNewReceiver = true;
            }

            if(receiverContainsSender == null)
            {
                receiver.Contacts.Add(new Contact
                {
                    User = sender,
                    HasNewMessage = true
                });
                result.IsNewSender = true;
            }
            else
            {
                receiverContainsSender.HasNewMessage = true;
            }

            Message msg = new Message
            {
                SenderId = senderNumericId,
                RecipientId = receiverNumericId,
                SendedAt = DateTimeHelper.GetMoscowTimestampNow(),
                Text = message
            };

            sender.MessagesSended.Add(msg);
            await _dbContext.SaveChangesAsync();

            result.Receiver = receiver;
            result.Sender = sender;

            return result;
        }
    }
}
