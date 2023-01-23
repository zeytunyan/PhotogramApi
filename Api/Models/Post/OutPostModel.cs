using Api.Models.Attach;
using Api.Models.User;

namespace Api.Models.Post
{
    public class OutPostModel
    {
        public Guid Id { get; set; }
        public string? Text { get; set; }
        public UserShortInfoModel Author { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Changed { get; set; }
        public Guid? RepostedId { get; set; }
        
        public int RepostsCount { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }

        public List<AttachExternalModel> Contents { get; set; } = null!;
    }
}
