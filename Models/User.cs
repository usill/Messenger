using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TestSignalR.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistredAt { get; set; }
        public List<User> Contacts { get; set; }
        public string Avatar { get; set; }
        public List<string> Connections { get; set; }
    }
}
