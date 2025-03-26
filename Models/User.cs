using System.ComponentModel.DataAnnotations.Schema;

namespace TestSignalR.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistredAt { get; set; }
    }
}
