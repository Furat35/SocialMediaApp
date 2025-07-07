using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Models.Followers
{
    public class FollowerListDto
    {
        public int Id { get; set; }
        public int RequestingUserId { get; set; }
        public int RespondingUserId { get; set; }
        //public FollowStatus Status { get; set; }
        public UserListDto User { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
