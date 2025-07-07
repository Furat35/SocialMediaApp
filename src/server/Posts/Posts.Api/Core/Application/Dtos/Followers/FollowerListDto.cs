using Posts.Api.Core.Domain.Enums;

namespace Posts.Api.Core.Application.Dtos.Followers
{
    public class FollowerListDto
    {
        public int Id { get; set; }
        public int RequestingUserId { get; set; }
        public int RespondingUserId { get; set; }
        public FollowStatus Status { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
