using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Core.Application.Dtos.Posts
{
    public class PostListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Fullname { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime CreateDate { get; set; }
        public ICollection<LikeListDto> Likes { get; set; }
        public ICollection<CommentListDto> Comments { get; set; }
    }
}
