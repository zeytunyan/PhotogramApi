 namespace DAL.Entities
{
    public class PostContent : Attach
    {
        public Guid PostId { get; set; }
        public virtual Post? Post { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
