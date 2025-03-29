namespace TestSignalR.Models.DTO
{
    public class GetContactRequest
    {
        public UserRequest recipient { get; set; }
        public List<MessageRequest> linkedMessages { get; set; }
    }
}
