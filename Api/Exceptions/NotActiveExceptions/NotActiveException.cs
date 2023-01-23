namespace Api.Exceptions.NotActiveExceptions
{
    public class NotActiveException : ItemException
    {
        public override string Message => $"{Item} is not active";
    }
}
