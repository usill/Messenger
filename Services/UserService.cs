﻿using Microsoft.EntityFrameworkCore;
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
    }
}
