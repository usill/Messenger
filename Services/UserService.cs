using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Models.DTO.response;
using TestSignalR.Models.Helper;
using TestSignalR.Services.Enums;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly IServiceProvider _serviceProvicer;
        public UserService(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvicer = serviceProvider;
        }
        public async Task<User?> FindByLoginAsync(string login)
        {
            return await _dbContext.Users.Where(u => u.Login == login).FirstOrDefaultAsync();
        }
        public async Task<User?> FindByIdAsync(int id)
        {
            return await _dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }
        public async Task<UserViewData?> GetViewDataAsync(int userId)
        {
            UserViewData? user = await _dbContext.Users
                                            .Include((u) => u.Contacts)
                                                .ThenInclude((c) => c.User)
                                            .Select((u) => new UserViewData { Id = u.Id, Avatar = u.Avatar, Username = u.Username, Contacts = u.Contacts })
                                            .Where(u => u.Id == userId)
                                            .FirstOrDefaultAsync();
            
            if(user == null)
            {
                return null;
            }

            user.PreparedContacts = await GetContactsAsync(user.Contacts, user.Id);

            return user;
        }
        public async Task ClearNotifyContact(int senderId, int recipientId)
        {
            Contact? contact = await _dbContext.Contacts
                                        .Include(c => c.User)
                                        .Where(c => c.OwnerId == senderId && c.User.Id == recipientId)
                                        .FirstOrDefaultAsync();

            if(contact == null)
            {
                return;
            }

            contact.HasNewMessage = false;
            await _dbContext.SaveChangesAsync();
        }
        public async Task SetStatusAsync(int userId, UserStatus status)
        {
            User? user = await _dbContext.Users
                                        .Where(u => u.Id == userId)
                                        .FirstOrDefaultAsync();

            if (user == null) return;

            user.Status = status;
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<string>> GetUserContactIdsAsync(int userId)
        {
            var contacts = await _dbContext.Contacts
                                            .Select(u => new { Id = u.Id, OwnerId = u.OwnerId })
                                            .Where(u => u.OwnerId == userId)
                                            .ToListAsync();

            if (contacts == null) return new List<string>();

            List<string> result = new List<string>();

            foreach(var c in contacts)
            {
                result.Add(c.Id.ToString());
            }

            return result;
        }
        private async Task<List<ContactResponse>> GetContactsAsync(List<Contact> contacts, int userId)
        {
            var messageService = _serviceProvicer.GetRequiredService<IMessageService>();
            List<ContactResponse> result = new List<ContactResponse>();

            foreach (Contact contact in contacts)
            {
                List<Message>? msg = await messageService.GetMessagesByUserAsync(userId, contact.User.Id, limit: 1);

                result.Add(new ContactResponse
                {
                    user = Mapper.Map<User, UserResponse>(contact.User),
                    linkedMessage = Mapper.Map<Message, MessageResponse>(msg.Last()),
                    hasNewMessage = contact.HasNewMessage,
                });
            }

            return result;
        }
    }
}
