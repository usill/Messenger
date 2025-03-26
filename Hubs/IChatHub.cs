namespace TestSignalR.Hubs
{
    public interface IChatHub
    {
        Task ReceiveMessage(string user, string message);
    }
}
