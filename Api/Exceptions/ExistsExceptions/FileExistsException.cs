using Common.Consts;

namespace Api.Exceptions.ExistsExceptions
{
    public class FileExistsException : ExistsException
    {
        public FileExistsException()
        {
            Item = ItemNames.File;
        }
    }
}
