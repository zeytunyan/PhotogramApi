namespace DAL.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string NickName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string GivenName { get; set; } = null!;
        public string? Surname { get; set; }
        public string FullName { get; set; } = null!;
        public DateTimeOffset BirthDate { get; set; }
        public string? Profession { get; set; }
        public string? Status { get; set; }
        public string Country { get; set; } = null!;
        public string Gender { get; set; } = null!;

        public bool IsDeleted { get; set; } = false; 
        public bool IsPrivate { get; set; } = false;

        public virtual Avatar? Avatar { get; set; }
        public virtual ICollection<UserSession>? Sessions { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
        public virtual ICollection<CommentLike>? CommentLikes { get; set; }
        public virtual ICollection<PostLike>? PostLikes { get; set; }

        public virtual ICollection<Following>? Followers { get; set; }
        public virtual ICollection<Following>? Followings { get; set; }

        public string? PushToken { get; set; }
    }
}