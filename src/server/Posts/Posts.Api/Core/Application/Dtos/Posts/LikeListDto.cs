namespace Posts.Api.Core.Application.Dtos.Posts
{
    public class LikeListDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
    }
}
