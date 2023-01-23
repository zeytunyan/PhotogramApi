using Common.Consts;

namespace Api.Exceptions.NotFoundExceptions
{
    public class SessionNotFoundException : NotFoundException
    {
        public SessionNotFoundException()
        {
            Item = ItemNames.Session;
        }
    }
}
