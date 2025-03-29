namespace TestSignalR.Models.DTO
{
    public class UserRequest
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public List<string> Connections { get; set; }
    }
}
