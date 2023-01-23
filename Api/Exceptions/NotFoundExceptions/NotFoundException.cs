using Common.Consts;

namespace Api.Exceptions.NotFoundExceptions
{
    public class NotFoundException : ItemException
    {
        public override string Message => $"{Item} not found";
    }
}
