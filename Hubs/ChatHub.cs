using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TestSignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }
        public override async Task OnConnectedAsync()
        {
            await Clients.All.ReceiveMessage(Context.ConnectionId, " Пользователь подключился");
        }
    }
}
