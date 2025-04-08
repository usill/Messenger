using Microsoft.EntityFrameworkCore;
using TestSignalR.Models;

namespace TestSignalR
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(u => u.Contacts)
                .WithOne(c => c.Owner)
                .HasForeignKey(c => c.OwnerId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.MessagesSended)
                .HasForeignKey(m => m.SenderId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.MessagesReceive)
                .HasForeignKey(m => m.RecipientId);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Token)
                .WithOne(t => t.User)
                .HasForeignKey<RefreshToken>(t => t.UserId);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
