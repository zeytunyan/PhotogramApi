namespace Api.Exceptions.ExistsExceptions
{
    public class ExistsException : ItemException
    {
        public override string Message => $"{Item} exists";
    }
}
