using System.ComponentModel.DataAnnotations.Schema;

namespace TestSignalR.Models
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public long SendedAt { get; set; }
        public string Text { get; set; }

    }
}
