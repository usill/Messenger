namespace TestSignalR.Models.DTO
{
    public class GetContactsRequest
    {
        public UserRequest recipient { get; set; }
        public MessageRequest linkedMessage { get; set; }
        public bool HasNewMessage { get; set; }
    }
}
