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

            if(result.IsNewContact)
            {
                await Clients.User(receiverId).AddContact(result.Sender.Username, result.Sender.Avatar, message);
                await Clients.User(senderId).AddContact(result.Receiver.Username, result.Receiver.Avatar, message);
            }

            await Clients.User(receiverId).ReceiveMessage(result.Sender.Username, message);

            await Clients.User(receiverId).UpdateContact(result.Sender.Username, message);
            await Clients.User(senderId).UpdateContact(result.Receiver.Username, message);
        }
    }
}
