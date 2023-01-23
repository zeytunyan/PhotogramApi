using Api.Models.Attach;
using Api.Services;
using AutoMapper;
using Common;
using Common.Consts;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class AttachController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly PostService _postService;
        private readonly UserService _userService;
        private readonly FileService _fileService;
        private readonly CheckService _checkService;

        public AttachController(
            PostService postService, 
            UserService userService, 
            FileService fileService, 
            CheckService checkService,
            IMapper mapper)
        {
            _fileService = fileService;
            _mapper = mapper;
            _postService = postService;
            _userService = userService;
            _checkService= checkService;
        }

        [HttpPost]
        public async Task<List<InFileMetaModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            var metas = new List<InFileMetaModel>();
            
            foreach (var file in files)
                metas.Add(await UploadFile(file)); 

            return metas;
        }

        private async Task<InFileMetaModel> UploadFile(IFormFile file)
        {
            var meta = _mapper.Map<InFileMetaModel>(file);
            await _fileService.SaveToTempAsync(file, meta.TempId.ToString());
            return meta;
        }

        // FileStreamResult? не нравится
        [HttpGet]
        public async Task<ActionResult> GetCurentUserAvatar(bool download = false)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await GetUserAvatar(userId, download);
        }

        // Сделать под авторизацией
        [AllowAnonymous]
        [HttpGet]
        [Route("{userId}")]
        public async Task<ActionResult> GetUserAvatar(Guid userId, bool download = false)
        {
            var avatar = await _userService.GetUserAvatarAsync(userId);
            return avatar == null ? Ok(null) : RenderAttach(avatar, download);
        }

        // Сделать под авторизацией
        [AllowAnonymous]
        [HttpGet]
        [Route("{postContentId}")]
        public async Task<FileStreamResult> GetPostContent(Guid postContentId, bool download = false)
        {
            //// Что если контента нет?
            var postContent = await _postService.GetPostContentAsync(postContentId);
            return RenderAttach(postContent, download);
        }

        private FileStreamResult RenderAttach(AttachModel attach, bool download)
        {
            _checkService.GetFileInfoOrThrowExIfNotExist(attach.FilePath);
            var extension = Path.GetExtension(attach.Name);

            try 
            {
                var fileStram = new FileStream(attach.FilePath, FileMode.Open);
                return download ? File(fileStram, attach.MimeType, $"{attach.Id}{extension}") : File(fileStram, attach.MimeType);
            }
            catch (IOException)
            {
                Thread.Sleep(100);
                return RenderAttach(attach, download);
            }
        }
    }
}
