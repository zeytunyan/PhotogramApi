namespace Api.Models.Comment
{
    public class InCommentWithAuthorModel : InCommentModel
    {
        public Guid AuthorId { get; set; }
    }
}
