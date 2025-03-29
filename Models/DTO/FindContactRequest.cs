namespace TestSignalR.Models.DTO
{
    public class FindContactRequest
    {
        public UserRequest recipient { get; set; }
        public List<MessageRequest> linkedMessages { get; set; }
    }
}
