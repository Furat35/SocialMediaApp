using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Models.Chats
{
    public class MessageListDto
    {
        public int Id { get; set; }
        public int From { get; set; }
        public int To { get; set; }
        public UserListDto User { get; set; }
        public string UserMessage { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime SentDate { get; set; }
    }
}
