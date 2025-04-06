namespace TestSignalR.Models.DTO
{
    public class FindContactResponse
    {
        public bool isFind { get; set; }
        public UserRequest user { get; set; } = new UserRequest();
        public List<MessageRequest> linkedMessages { get; set; } = new List<MessageRequest>();
    }
}
