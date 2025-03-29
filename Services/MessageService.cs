using Microsoft.EntityFrameworkCore;
using TestSignalR.Models;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _dbContext;
        public MessageService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Message>> GetMessagesByUserAsync(int recipientId, int senderId)
        {
            return await _dbContext.Messages
                .FromSqlInterpolated($"SELECT * FROM Messages WHERE RecipientId = {recipientId} AND SenderId = {senderId} OR SenderId = {recipientId} AND RecipientId = {senderId} ORDER BY SendedAt ASC")
                .ToListAsync();
        }
    }
}
