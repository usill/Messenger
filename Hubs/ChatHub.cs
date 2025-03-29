using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using TestSignalR.Models;

namespace TestSignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private readonly AppDbContext _dbContext;
        public ChatHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SendMessage(string receiverId, string message)
        {
            int senderId = Convert.ToInt32(Context.User?.FindFirstValue(ClaimTypes.NameIdentifier));
            int receiverNumericId;
            User? sender = _dbContext.Users.Where(u => u.Id == senderId).FirstOrDefault();

            if(!Int32.TryParse(receiverId, out receiverNumericId))
            {
                return;
            }

            if(sender == null)
            {
                return;
            }

            Message msg = new Message
            {
                SenderId = senderId,
                RecipientId = receiverNumericId,
                SendedAt = DateTime.UtcNow,
                Text = message
            };

            sender.MessagesSended.Add(msg);
            await _dbContext.SaveChangesAsync();

            await Clients.User(receiverId).ReceiveMessage(message);
        }
    }
}
