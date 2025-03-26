using Microsoft.EntityFrameworkCore;
using TestSignalR.Models;

namespace TestSignalR
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
        public DbSet<User> Users { get; set; }
    }
}
