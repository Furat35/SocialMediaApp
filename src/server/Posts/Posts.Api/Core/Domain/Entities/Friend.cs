using BuildingBlocks.Models;
using Posts.Api.Core.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posts.Api.Core.Domain.Entities
{
    public class Friend : BaseEntity
    {
        public int RequestingUserId { get; set; }
        public int RespondingUserId { get; set; }
        public FriendStatus Status { get; set; }
        [NotMapped]
        public override bool IsValid { get; set; }
    }
}
