namespace TestSignalR.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(string login, string message);
        Task AddContact(string contact);
        Task UpdateContact(string login, string lastMessage);
        Task ReceiveContact(string contact);
        Task UserOnline(string login);
        Task UserOffline(string login);
    }
}
