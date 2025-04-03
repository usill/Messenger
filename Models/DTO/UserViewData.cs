namespace TestSignalR.Models.DTO
{
    public class UserViewData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public List<User> Contacts { get; set; }
        public List<GetContactsRequest> PreparedContacts { get; set; }
    }
}
