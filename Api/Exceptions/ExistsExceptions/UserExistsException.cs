using Common.Consts;

namespace Api.Exceptions.ExistsExceptions
{
    public class UserExistsException : ExistsException
    {
        public UserExistsException()
        {
            Item = ItemNames.User;
        }
    }
}
