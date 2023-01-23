namespace Api.Models.Comment
{
    public class InCommentModel
    {
        public Guid PostId { get; set; }
        public string Text { get; set; } = null!;

        public Guid? ParentId { get; set; }

    }
}
