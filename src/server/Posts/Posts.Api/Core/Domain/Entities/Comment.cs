using BuildingBlocks.Models;

namespace Posts.Api.Core.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string UserComment { get; set; }
        public int UserId { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
    }
}
