using Microsoft.AspNetCore.Authentication.OAuth.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Immutable;
using System.Reflection;
using System.Security.Claims;
using System.Xml.Linq;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Models.DTO.response;
using TestSignalR.Models.Helper;
using TestSignalR.Services.Enums;
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
            string? senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return;

            SendMessageResponse? result = await _messageService.SendMessage(receiverId, senderId, message);

            if(result == null)
            {
                return;
            }

            if(result.IsNewSender)
            {
                var contact = new ContactResponse
                {
                    user = Mapper.Map<User, UserResponse>(result.Sender),
                    linkedMessage = new MessageResponse
                    {
                        Text = message,
                    },
                    hasNewMessage = true,
                };
                await Clients.User(receiverId).AddContact(JsonHelper.Serialize(contact));
            }
            if(result.IsNewReceiver)
            {
                var contact = new ContactResponse
                {
                    user = Mapper.Map<User, UserResponse>(result.Receiver),
                    linkedMessage = new MessageResponse
                    {
                        Text = message,
                    },
                    hasNewMessage = true,
                };
                await Clients.User(senderId).AddContact(JsonHelper.Serialize(contact));
            }

            await Clients.User(receiverId).ReceiveMessage(result.Sender.Login, message);

            await Clients.User(receiverId).UpdateContact(result.Sender.Login, message);
            await Clients.User(senderId).UpdateContact(result.Receiver.Login, message);
            return;
        }
        public async Task FindContact(string login)
        {
            string? senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return;

            FindContactResponse result = new FindContactResponse();
            result.isFind = false;

            User? recipient = await _userService.FindByNameAsync(login);

            if (recipient == null)
            {
                await Clients.User(senderId).ReceiveContact(JsonHelper.Serialize(result));
                return;
            }

            List<Message> linkedMessages = await _messageService.GetMessagesByUserAsync(recipient.Id, Int32.Parse(senderId));
            List<MessageResponse> messagesRequest = new List<MessageResponse>();

            foreach (var msg in linkedMessages)
            {
                messagesRequest.Add(Mapper.Map<Message, MessageResponse>(msg));
            }

            result.isFind = true;
            result.user = Mapper.Map<User, UserResponse>(recipient);
            result.linkedMessages = messagesRequest;

            await _userService.ClearNotifyContact(Int32.Parse(senderId), recipient.Id);

            await Clients.User(senderId).ReceiveContact(JsonHelper.Serialize(result));
            return;
        }
        public async Task GetMessages(string reseiverId, string pageIndex)
        {

            string? senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return;

            int recipientId, index;

            bool isRecipientNumeric = Int32.TryParse(reseiverId, out recipientId);
            bool isPageIndexNumeric = Int32.TryParse(pageIndex, out index);

            if (!isRecipientNumeric || !isPageIndexNumeric)
            {
                return;
            }

            List<Message> messages = await _messageService.GetMessagesByUserAsync(recipientId, Int32.Parse(senderId), index: index);

            await Clients.User(senderId).DrawMoreMessages(JsonHelper.Serialize(messages));
        }
        public override async Task OnConnectedAsync()
        {
            string? senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return;
            await _userService.SetStatusAsync(Int32.Parse(senderId), UserStatus.Online);
            List<string> contacts = await _userService.GetUserContactIdsAsync(Int32.Parse(senderId));
            string login = Context.User.FindFirstValue(ClaimTypes.Name);
            await Clients.Users(contacts).UserOnline(login);
            await base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string? senderId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (senderId == null) return;
            await _userService.SetStatusAsync(Int32.Parse(senderId), UserStatus.Offline);
            List<string> contacts = await _userService.GetUserContactIdsAsync(Int32.Parse(senderId));
            string login = Context.User.FindFirstValue(ClaimTypes.Name);
            await Clients.Users(contacts).UserOffline(login);
            await base.OnDisconnectedAsync(exception);
        }
    }
}
