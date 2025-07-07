using Posts.Api.Core.Domain.Entities;

namespace Posts.Api.Core.Application.Dtos.Posts
{
    public class CommentListDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserComment { get; set; }
        public int PostId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
