namespace TestSignalR.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(string message);
        Task AddContact(string contactName, string avatar, string lastMessage);
    }
}
