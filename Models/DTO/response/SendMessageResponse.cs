namespace TestSignalR.Models.DTO.response
{
    public class SendMessageResponse
    {
        public bool IsNewReceiver { get; set; }
        public bool IsNewSender { get; set; }
        public User Sender { get; set; }
        public User Receiver { get; set; }
    }
}
