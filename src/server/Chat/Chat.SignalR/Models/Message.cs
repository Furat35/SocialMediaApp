namespace Chat.SignalR.Models
{
    public class Message
    {
        public Message(int from, int to, string userMessage)
        {
            From = from;
            To = to;
            UserMessage = userMessage;
        }
        public Message()
        {

        }

        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public string UserMessage { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime SentDate { get; set; } = DateTime.UtcNow;
    }
}
