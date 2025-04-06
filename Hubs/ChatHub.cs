using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using System.Xml.Linq;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Models.Helper;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        public ChatHub(AppDbContext dbContext, IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }
        public async Task SendMessage(string receiverId, string message)
        {
            string? senderId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
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

            await Clients.User(receiverId).ReceiveMessage(result.Sender.Login, message);

            await Clients.User(receiverId).UpdateContact(result.Sender.Login, message);
            await Clients.User(senderId).UpdateContact(result.Receiver.Login, message);
            return;
        }
        public async Task FindContact(string login)
        {
            string? sender = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (sender == null) return;
            int senderId = Int32.Parse(sender);

            FindContactResponse result = new FindContactResponse();
            result.isFind = false;

            User? recipient = await _userService.FindByNameAsync(login);

            if (recipient == null)
            {
                await Clients.User(sender).ReceiveContact(JsonHelper.Serialize(result));
                return;
            }

            List<Message> linkedMessages = await _messageService.GetMessagesByUserAsync(recipient.Id, senderId);
            List<MessageRequest> messagesRequest = new List<MessageRequest>();

            foreach (var msg in linkedMessages)
            {
                messagesRequest.Add(Mapper.Map<Message, MessageRequest>(msg));
            }

            result.isFind = true;
            result.user = Mapper.Map<User, UserRequest>(recipient);
            result.linkedMessages = messagesRequest;

            await _userService.ClearNotifyContact(senderId, recipient.Id);

            await Clients.User(sender).ReceiveContact(JsonHelper.Serialize(result));
            return;
        }
    }
}
