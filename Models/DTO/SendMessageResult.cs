namespace TestSignalR.Models.DTO
{
    public class SendMessageResult
    {
        public bool IsNewContact { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
