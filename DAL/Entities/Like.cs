namespace DAL.Entities
{
    public class Like
    {
        public Guid UserId { get; set; }
        public virtual User? User { get; set; }
        
        public DateTimeOffset Date { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
