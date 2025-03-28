using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestSignalR.Models;

namespace TestSignalR.Hubs
{
    [Authorize]
    public class ChatHub : Hub<IChatHub>
    {
        private readonly AppDbContext _dbContext;
        public ChatHub(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task SendMessage(string connections, string message)
        {
            Console.WriteLine(connections);
            await Clients.All.ReceiveMessage(message);
        }
        public override async Task OnConnectedAsync()
        {
            string? id = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (id == null)
            {
                return;
            }

            int userId = Convert.ToInt32(id);

            User? user = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();

            if(user == null)
            {
                return;
            }

            user.Connections.Add(Context.ConnectionId);
            await _dbContext.SaveChangesAsync();
            await base.OnConnectedAsync();
        }
    }
}
