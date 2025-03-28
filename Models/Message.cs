using System.ComponentModel.DataAnnotations.Schema;

namespace TestSignalR.Models
{
    public class Message
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("SenderId")]
        public int SenderId { get; set; }
        public User Sender { get; set; }
        [ForeignKey("RecipientId")]
        public int RecipientId { get; set; }
        public User Recipient { get; set; }
        public DateTime SendedAt { get; set; }
        public string Text { get; set; }

    }
}
