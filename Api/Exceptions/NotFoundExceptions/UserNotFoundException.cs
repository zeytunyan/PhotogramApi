using Common.Consts;

namespace Api.Exceptions.NotFoundExceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException()
        {
            Item = ItemNames.User;
        }

    }
}
