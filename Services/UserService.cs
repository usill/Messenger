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
        public async Task<List<GetContactsRequest>> GetContactsAsync(int userId)
        {
            User? currentUser = await _dbContext.Users.Include(u => u.Contacts).Where(u => u.Id == userId).FirstOrDefaultAsync();
            List<GetContactsRequest> result = new List<GetContactsRequest>();

            if(currentUser == null)
            {
                return new List<GetContactsRequest> { };
            }

            foreach(User contact in currentUser.Contacts)
            {
                var messageService = _serviceProvicer.GetRequiredService<IMessageService>();
                List<Message> msg = await messageService.GetMessagesByUserAsync(currentUser.Id, contact.Id, 1);
                result.Add(new GetContactsRequest
                {
                    recipient = Mapper.Map<User, UserRequest>(contact),
                    linkedMessage = Mapper.Map<Message, MessageRequest>(msg.Last())
                });
            }

            return result;
        }
        public async Task<string> GetAvatar(int userId)
        {
            User? currentUser = await _dbContext.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            
            if(currentUser == null)
            {
                return "";
            }

            return currentUser.Avatar;
        }
        public async Task<User?> FindByNameAsync(string username)
        {
            return await _dbContext.Users.Where(u => u.Username == username).FirstOrDefaultAsync();
        }
        public async Task<User?> FindByIdAsync(int id)
        {
            return await _dbContext.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
        }
        public async Task AddContactAsync(User u1, User u2)
        {
            User? contact1 = u1.Contacts.Find(c => c.Id == u2.Id);
            
            if(contact1 == null)
            {
                u1.Contacts.Add(u2);
            }

            User? contact2 = u2.Contacts.Find(c => c.Id == u1.Id);

            if(contact2 == null)
            {
                u2.Contacts.Add(u1);
            }
            
            await _dbContext.SaveChangesAsync();
        }
    }
}
