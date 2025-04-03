﻿using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Models.Helper;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.Services
{
    public class MessageService : IMessageService
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;
        public MessageService(AppDbContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }
        public async Task<List<Message>> GetMessagesByUserAsync(int recipientId, int senderId, int limit = 25, string order = "ASC")
        {
            string safeOrder = order.ToUpper() == "DESC" ? "DESC" : "ASC";
            string sql = $@"
                            SELECT * FROM Messages 
                            WHERE RecipientId = {recipientId} AND SenderId = {senderId} 
                            OR SenderId = {recipientId} AND RecipientId = {senderId} 
                            ORDER BY SendedAt {safeOrder} 
                            LIMIT {limit}";

            return await _dbContext.Messages.FromSqlRaw(sql).ToListAsync();
        }
        public async Task<SendMessageResult?> SendMessage(string receiverId, string senderId, string message)
        {
            SendMessageResult result = new SendMessageResult();
            result.IsNewReceiver = false;
            result.IsNewSender = false;
            int senderNumericId = Convert.ToInt32(senderId);
            int receiverNumericId;

            if (!Int32.TryParse(receiverId, out receiverNumericId))
            {
                return null;
            }

            User? sender = _dbContext.Users.Where(u => u.Id == senderNumericId).FirstOrDefault();
            User? receiver = await _userService.FindByIdAsync(receiverNumericId);

            if (sender == null || receiver == null)
            {
                return null;
            }

            User? senderContainsRceiver = await _dbContext.Users.Include(u => u.Contacts).Where(u => u.Contacts.Contains(receiver) && u.Id == senderNumericId).FirstOrDefaultAsync();
            User? receiverContainsSender = await _dbContext.Users.Include(u => u.Contacts).Where(u => u.Contacts.Contains(sender) && u.Id == receiverNumericId).FirstOrDefaultAsync();

            if(senderContainsRceiver == null)
            {
                sender.Contacts.Add(receiver);
                result.IsNewReceiver = true;
            }
            if(receiverContainsSender == null)
            {
                receiver.Contacts.Add(sender);
                result.IsNewSender = true;
            }

            Message msg = new Message
            {
                SenderId = senderNumericId,
                RecipientId = receiverNumericId,
                SendedAt = DateTimeHelper.GetMoscowTimestampNow(),
                Text = message
            };

            sender.MessagesSended.Add(msg);
            await _dbContext.SaveChangesAsync();

            result.Receiver = receiver;
            result.Sender = sender;

            return result;
        }
    }
}
