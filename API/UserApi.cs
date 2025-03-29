using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TestSignalR.Services.Interfaces;
using TestSignalR.Models.Helper;
using TestSignalR.Models.DTO;
using TestSignalR.Models;
using System.Security.Claims;

namespace TestSignalR.API
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserApi : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;
        public UserApi(IUserService userService, IMessageService messageService)
        {
            _userService = userService;
            _messageService = messageService;
        }
        [HttpGet("find/{name}")]
        public async Task<ActionResult<GetContactRequest>> FindByName(string name)
        {
            int senderId = Convert.ToInt32(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User? recipient = await _userService.FindByNameAsync(name);

            if(recipient == null)
            {
                return NotFound();
            }

            List<Message> linkedMessages = await _messageService.GetMessagesByUserAsync(recipient.Id, senderId);
            List<MessageRequest> messagesRequest = new List<MessageRequest>();

            foreach(var msg in linkedMessages)
            {
                messagesRequest.Add(Mapper.Map<Message, MessageRequest>(msg));
            }

            GetContactRequest findResult = new GetContactRequest
            {
                recipient = Mapper.Map<User, UserRequest>(recipient),
                linkedMessages = messagesRequest,
            };

            return Ok(JsonHelper.Serialize(findResult));
        }
    }
}
