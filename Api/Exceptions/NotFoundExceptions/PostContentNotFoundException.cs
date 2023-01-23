using Common.Consts;

namespace Api.Exceptions.NotFoundExceptions
{
    public class PostContentNotFoundException : NotFoundException
    {
        public PostContentNotFoundException()
        {
            Item = ItemNames.PostContent;
        }
    }
}
