using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Models.Friends
{
    public class FriendListDto
    {
        public int UserId { get; set; }
        public UserListDto User { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
