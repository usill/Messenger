namespace TestSignalR.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(string username, string login, string message);
        Task AddContact(string contactName, string login, string avatar, string lastMessage);
        Task UpdateContact(string login, string lastMessage);
    }
}
