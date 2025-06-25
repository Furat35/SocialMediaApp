using SocialMediaApp.Aggregator.Models.Posts.Comments;
using SocialMediaApp.Aggregator.Models.Posts.Likes;
using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Models.Posts
{
    public class PostListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public UserListDto User { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<LikeListDto> Likes { get; set; }
        public ICollection<CommentListDto> Comments { get; set; }
    }
}
