using Common.Consts;

namespace Api.Exceptions.NotFoundExceptions
{
    public class FileNotFoundException : NotFoundException
    {
        public FileNotFoundException()
        {
            Item = ItemNames.File;
        }

    }
}
