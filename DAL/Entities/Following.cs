namespace DAL.Entities
{
    public class Following
    {
        public Guid FollowerId { get; set; }
        public virtual User? Follower { get; set; }

        public Guid FollowedToId { get; set; }
        public virtual User? FollowedTo { get; set; }

        public DateTimeOffset FollowDate { get; set; }
        public DateTimeOffset? UnfollowDate { get; set; }
    }
}
