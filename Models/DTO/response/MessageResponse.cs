namespace TestSignalR.Models.DTO.response
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public long SendedAt { get; set; }
        public string Text { get; set; }
        public int RecipientId { get; set; }
    }
}
