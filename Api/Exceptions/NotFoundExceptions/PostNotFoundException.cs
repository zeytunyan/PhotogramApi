using Common.Consts;

namespace Api.Exceptions.NotFoundExceptions
{
    public class PostNotFoundException : NotFoundException
    {
        public PostNotFoundException()
        {
            Item = ItemNames.Post;
        }
    }
}
