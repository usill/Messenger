using Microsoft.EntityFrameworkCore;
using TestSignalR.Models;
using TestSignalR.Services.Interfaces;

namespace TestSignalR.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<User>> GetContactsAsync(int userId)
        {
            User? currentUser = await _dbContext.Users.Include(u => u.Contacts).Where(u => u.Id == userId).FirstOrDefaultAsync();

            if(currentUser == null)
            {
                return new List<User> { };
            }

            return currentUser.Contacts;
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
