using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<UserSession> UserSessions => Set<UserSession>();
        public DbSet<Attach> Attaches => Set<Attach>();
        public DbSet<Avatar> Avatars => Set<Avatar>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<PostContent> PostContents => Set<PostContent>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<PostLike> PostLikes => Set<PostLike>();
        public DbSet<CommentLike> CommentLikes => Set<CommentLike>();
        public DbSet<Following> Followings => Set<Following>();

        public DataContext(DbContextOptions<DataContext> options) : base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Like>();
   
            modelBuilder.Entity<Avatar>().ToTable(nameof(Avatars));
            modelBuilder.Entity<PostContent>().ToTable(nameof(PostContents));
            modelBuilder.Entity<PostLike>().ToTable(nameof(PostLikes));
            modelBuilder.Entity<CommentLike>().ToTable(nameof(CommentLikes));

            modelBuilder.Entity<Following>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Followings)
                .HasForeignKey(f => f.FollowerId)
                .HasConstraintName("FK_Followings_Users_FollowerId");

            modelBuilder.Entity<Following>()
                .HasOne(f => f.FollowedTo)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FollowedToId)
                .HasConstraintName("FK_Followings_Users_FollowedToId");

            modelBuilder.Entity<User>().HasAlternateKey(u => u.NickName);
            modelBuilder.Entity<User>().HasAlternateKey(u => u.Email);
            modelBuilder.Entity<User>().HasAlternateKey(u => u.PhoneNumber);
            modelBuilder.Entity<Post>().HasAlternateKey(p => new { p.AuthorId, p.Created });
            modelBuilder.Entity<Comment>().HasAlternateKey(c => new { c.AuthorId, c.PostId, c.Created });

            modelBuilder.Entity<PostLike>().HasKey(pl => new { pl.UserId, pl.PostId, pl.Date });
            modelBuilder.Entity<CommentLike>().HasKey(cl => new { cl.UserId, cl.CommentId, cl.Date });
            modelBuilder.Entity<Following>().HasKey(f => new { f.FollowerId, f.FollowedToId, f.FollowDate });

            modelBuilder.Entity<User>().Property(u => u.FullName).HasComputedColumnSql(
                "\"Users\".\"GivenName\" || ' ' || \"Users\".\"Surname\"", true);

            modelBuilder.Entity<User>().HasCheckConstraint(
                "\"Users\".\"Gender\"", "\"Users\".\"Gender\" IN ('Man', 'Woman', 'Another', 'man', 'woman', 'another')"); // 🏳️‍🌈
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) 
            => optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("Api")); // Сборка, в которой хранятся миграции
    }
}
