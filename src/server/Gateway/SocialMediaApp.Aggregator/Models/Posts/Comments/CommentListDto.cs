using SocialMediaApp.Aggregator.Models.Users;

namespace SocialMediaApp.Aggregator.Models.Posts.Comments
{
    public class CommentListDto
    {
        public int Id { get; set; }
        public string UserComment { get; set; }
        public int UserId { get; set; }
        public UserListDto User { get; set; }
        public int PostId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
