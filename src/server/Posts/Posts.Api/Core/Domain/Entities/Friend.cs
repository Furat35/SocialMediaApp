using BuildingBlocks.Models;

namespace Posts.Api.Core.Domain.Entities
{
    public class Friend : BaseEntity
    {
        public int RequestingUserId { get; set; }
        public int RespondingUserId { get; set; }
        public bool IsValid { get; set; }
    }
}
