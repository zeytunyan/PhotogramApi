using Common.Consts;

namespace Api.Exceptions.NotActiveExceptions
{
    public class SessionNotActiveException : NotActiveException
    {
        public SessionNotActiveException()
        {
            Item = ItemNames.Session;
        }

    }
}
