using Microsoft.EntityFrameworkCore;
using TestSignalR.Models;
using TestSignalR.Models.DTO;
using TestSignalR.Models.Helper;
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
        public async Task<User?> FindByNameAsync(string username)
        {
            return await _dbContext.Users.Where(u => u.Login == username).FirstOrDefaultAsync();
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
        private async Task<List<GetContactsRequest>> GetContactsAsync(List<Contact> contacts, int userId)
        {
            var messageService = _serviceProvicer.GetRequiredService<IMessageService>();
            List<GetContactsRequest> result = new List<GetContactsRequest>();

            foreach (Contact contact in contacts)
            {
                List<Message>? msg = await messageService.GetMessagesByUserAsync(userId, contact.User.Id, 1, "DESC");

                result.Add(new GetContactsRequest
                {
                    recipient = Mapper.Map<User, UserRequest>(contact.User),
                    linkedMessage = Mapper.Map<Message, MessageRequest>(msg.Last()),
                    HasNewMessage = contact.HasNewMessage,
                });
            }

            return result;
        }

    }
}
