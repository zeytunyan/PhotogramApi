namespace Api.Exceptions
{
    public class TooManyRequestsException : Exception 
    {
        public override string Message => "Too many requests";
    }
}
