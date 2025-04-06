using TestSignalR.Models.DTO.response;

namespace TestSignalR.Models.DTO
{
    public class ContactResponse
    {
        public UserResponse user { get; set; }
        public MessageResponse linkedMessage { get; set; }
        public bool hasNewMessage { get; set; } = false;
    }
}
