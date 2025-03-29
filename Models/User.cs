using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using TestSignalR.Models;

namespace TestSignalR.Models
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public DateTime RegistredAt { get; set; }
        public List<User> Contacts { get; set; } = new List<User> { };
        public string Avatar { get; set; }
        public List<Message> MessagesSended { get; set; } = new List<Message> { };
        public List<Message> MessagesReceive { get; set; } = new List<Message> { };
    }
}
