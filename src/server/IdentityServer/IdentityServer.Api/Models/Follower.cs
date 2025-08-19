using BuildingBlocks.Models;
using IdentityServer.Api.Core.Domain.Enums;

namespace IdentityServer.Api.Core.Domain.Entities
{
    public class Follower : BaseEntity
    {
        public int RequestingUserId { get; set; }
        public int RespondingUserId { get; set; }
        public FollowStatus Status { get; set; }
        public override bool IsValid { get; set; }
    }
}
