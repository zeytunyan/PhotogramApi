namespace DAL.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public bool IsDeleted { get; set; } = false;

        public Guid PostId { get; set; }
        public virtual Post? Post { get; set; }

        public Guid AuthorId { get; set; }
        public virtual User? Author { get; set; }

        public Guid? ParentId { get; set; }
        public virtual Comment? ParentComment { get; set; }

        public virtual ICollection<Comment>? Children { get; set; }
        
        public virtual ICollection<CommentLike>? Likes { get; set; }
    }
}
