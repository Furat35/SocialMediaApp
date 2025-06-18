using BuildingBlocks.Models;

namespace Posts.Api.Core.Domain.Entities
{
    public class Like : BaseEntity
    {
        public int PostId { get; set; }
        public Post Post { get; set; }
        public int UserId { get; set; }
    }
}
