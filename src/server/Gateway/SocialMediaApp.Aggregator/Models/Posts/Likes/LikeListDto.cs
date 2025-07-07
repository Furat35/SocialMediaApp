using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Models.Posts.Likes
{
    public class LikeListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserListDto User { get; set; }
        public int PostId { get; set; }
    }
}
