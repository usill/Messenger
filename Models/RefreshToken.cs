using System.ComponentModel.DataAnnotations.Schema;

namespace TestSignalR.Models
{
    public class RefreshToken
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string HashValue { get; set; }
        public DateTime ExpiresAt { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
