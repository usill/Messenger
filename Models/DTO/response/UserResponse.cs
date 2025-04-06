using TestSignalR.Services.Enums;

namespace TestSignalR.Models.DTO.response
{
    public class UserResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Login { get; set; }
        public string Avatar { get; set; }
        public UserStatus Status { get; set; }
    }
}
