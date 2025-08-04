using BuildingBlocks.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Posts.Api.Core.Domain.Entities
{
    public class Story : BaseEntity
    {
        public string ImagePath { get; set; }
        public int UserId { get; set; }
        public override DateTime CreateDate { get; set; }
    }
}
