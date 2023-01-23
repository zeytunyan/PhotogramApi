using Common.Consts;

namespace Api.Exceptions.IncorrectExceptions
{
    public class PasswordIncorrectException : IncorrectException
    {
        public PasswordIncorrectException()
        {
            Item = ItemNames.Password;
        }
    }
}
