namespace Api.Services
{
    public class FileService
    {
        private readonly IConfiguration _configuration;
        private readonly CheckService _checkService;

        public FileService(IConfiguration configuration, CheckService checkService)
        {
            _configuration = configuration;
            _checkService = checkService;
        }

        public async Task<string> SaveToTempAsync(IFormFile file, string fileName)
        {
            var newPath = Path.Combine(Path.GetTempPath(), fileName);
            
            _checkService.ThrowExIfFileExist(newPath);

            using var stream = File.Create(newPath);
            await file.CopyToAsync(stream);
            return newPath;
        } 

        public string MoveToPermanentFolder(string fileName)
        {
            var tempPath = Path.Combine(Path.GetTempPath(), fileName);
            var tempFileInfo = _checkService.GetFileInfoOrThrowExIfNotExist(tempPath);
            var attachesFolder = _configuration.GetValue<string>("AttachesFolder");
            var path = Path.Combine(Directory.GetCurrentDirectory(), attachesFolder, fileName);

            var destFi = new FileInfo(path);

            if (destFi.Directory != null && !destFi.Directory.Exists)
                destFi.Directory.Create();

            File.Move(tempFileInfo.FullName, path, true);
            return path;
        }

    }
}
