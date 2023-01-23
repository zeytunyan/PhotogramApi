namespace DAL.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public DateTimeOffset Created { get; set; }
        public bool IsDeleted { get; set; } = false;
        public DateTimeOffset? Changed { get; set; }

        public Guid AuthorId { get; set; }
        public virtual User? Author { get; set; }

        public virtual ICollection<PostContent> Contents { get; set; } = null!;

        public Guid? RepostedId { get; set; }
        public virtual Guid? Reposted { get; set; }

        public virtual ICollection<Post>? Reposts { get; set; }
        public virtual ICollection<PostLike>? Likes { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
