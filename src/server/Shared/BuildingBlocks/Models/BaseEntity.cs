namespace BuildingBlocks.Models
{
    public class BaseEntity : IBaseEntity
    {
        public virtual int Id { get; set; }
        public virtual DateTime CreateDate { get; set; }
        public virtual DateTime ModifyDate { get; set; }
        public virtual bool IsValid { get; set; } = true;
    }
}
