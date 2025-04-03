using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private readonly AppDbContext _dbContext;
        private readonly IMessageService _messageService;
        public ChatHub(AppDbContext dbContext, IMessageService messageService)
        {
            _dbContext = dbContext;
            _messageService = messageService;
        }
        public async Task SendMessage(string receiverId, string message)
        {
            string senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            SendMessageResult? result = await _messageService.SendMessage(receiverId, senderId, message);

            if(result == null)
            {
                return;
            }

            if(result.IsNewSender)
            {
                await Clients.User(receiverId).AddContact(result.Sender.Username, result.Sender.Login, result.Sender.Avatar, message);
            }
            if(result.IsNewReceiver)
            {
                await Clients.User(senderId).AddContact(result.Sender.Username, result.Receiver.Login, result.Receiver.Avatar, message);
            }

            await Clients.User(receiverId).ReceiveMessage(result.Sender.Username, result.Sender.Login, message);

            await Clients.User(receiverId).UpdateContact(result.Sender.Login, message);
            await Clients.User(senderId).UpdateContact(result.Receiver.Login, message);
        }
    }
}
