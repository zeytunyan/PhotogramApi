namespace Api.Exceptions
{
    public class UnauthorizedException : Exception
    {
        public override string Message => "Unauthorized access attempt";
    }
}
