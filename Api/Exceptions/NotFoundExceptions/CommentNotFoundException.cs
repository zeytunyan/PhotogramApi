using Common.Consts;

namespace Api.Exceptions.NotFoundExceptions
{
    public class CommentNotFoundException : NotFoundException
    {
        public CommentNotFoundException()
        {
            Item = ItemNames.Comment;
        }
    }
}
