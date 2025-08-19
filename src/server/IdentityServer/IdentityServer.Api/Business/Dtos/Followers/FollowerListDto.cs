using IdentityServer.Api.Core.Domain.Enums;

namespace IdentityServer.Api.Business.Dtos.Followers
{
    public class FollowerListDto
    {
        public int? Id { get; set; }
        public int? RequestingUserId { get; set; }
        public int? RespondingUserId { get; set; }
        public FollowStatus Status { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
