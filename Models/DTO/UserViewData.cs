namespace TestSignalR.Models.DTO
{
    public class UserViewData
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public List<Contact> Contacts { get; set; }
        public List<ContactResponse> PreparedContacts { get; set; }
    }
}
