using Api.Models.User;
using DAL.Entities;

namespace Api.Models.Comment
{
    public class OutCommentModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        public Guid PostId { get; set; }
        public UserShortInfoModel? Author { get; set; } = null!;
        public Guid? ParentId { get; set; }

        public int ChildrenCount { get; set; }
        public int LikesCount { get; set; }
    }
}
