using System.ComponentModel.DataAnnotations.Schema;

namespace TestSignalR.Models
{
    public class Contact
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public User User { get; set; }
        public User Owner { get; set; }
        public int OwnerId { get; set; }
        public bool HasNewMessage { get; set; }
    }
}
