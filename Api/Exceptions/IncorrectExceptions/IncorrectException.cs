namespace Api.Exceptions.IncorrectExceptions
{
    public class IncorrectException : ItemException
    {
        public override string Message => $"{Item} is incorrect";
    }
}
