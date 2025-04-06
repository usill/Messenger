namespace TestSignalR.Models.DTO.response
{
    public class FindContactResponse
    {
        public bool isFind { get; set; }
        public UserResponse user { get; set; } = new UserResponse();
        public List<MessageResponse> linkedMessages { get; set; } = new List<MessageResponse>();
    }
}
