namespace TestSignalR.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(string username, string message);
        Task AddContact(string contactName, string avatar, string lastMessage);
        Task UpdateContact(string contactName, string lastMessage);
    }
}
