using BuildingBlocks.Models;

namespace Posts.Api.Core.Domain.Entities
{
    public class Post : BaseEntity
    {
        public int UserId { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string ImagePath { get; set; }
        public ICollection<Like> Likes { get; set; }
        public ICollection<Comment> Comments { get; set; }
    }
}
