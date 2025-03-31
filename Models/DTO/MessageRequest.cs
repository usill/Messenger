namespace TestSignalR.Models.DTO
{
    public class MessageRequest
    {
        public int Id { get; set; }
        public long SendedAt { get; set; }
        public string Text { get; set; }
        public int RecipientId { get; set; }
    }
}
