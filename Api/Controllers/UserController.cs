using Api.Models.Attach;
using Api.Models.User;
using Api.Services;
using Common.Consts;
using Common.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly FileService _fileService;
        private readonly CheckService _checkService;


        public UserController(
            UserService userService, 
            FileService fileService, 
            LinkGeneratorService links, 
            CheckService checkService)
        {
            _userService = userService;
            _fileService = fileService;
            _checkService = checkService;

            links.LinkAvatarGenerator = x =>
            Url.ControllerAction<AttachController>(nameof(AttachController.GetUserAvatar), new
            {
                userId = x.Id,
            });
        }
     
        [AllowAnonymous]
        [HttpGet]
        public async Task<List<UserShortInfoModel>> GetUsers() =>
            await _userService.GetUsersAsync();


        [HttpDelete]
        public async Task<UserShortInfoModel> Delete()
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            var deletionRes = await _userService.DeleteUserAsync(userId);
            //// Добавить отправку письма об удалении
            return deletionRes;
        }

        [HttpPost]
        public async Task<Guid> AddAvatarToUser(InFileMetaModel model)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            var moveToPermPath = _fileService.MoveToPermanentFolder(model.TempId.ToString());

            return await _userService.AddUserAvatarAsync(userId, model, moveToPermPath);
        }

        [HttpGet]
        public async Task<UserPageInfoModel> GetCurrentUser()
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await GetUserById(userId);
        }

        [HttpGet]
        public async Task<UserPageInfoModel> GetUserById(Guid userId)
        {
            return await _userService.GetUserPageInfoAsync(userId);
        }

        [HttpGet]
        public async Task<UserFullInfoModel> GetCurrentUserFullInfo()
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await GetUserFullInfoById(userId);
        }

        //[HttpGet]
        private async Task<UserFullInfoModel> GetUserFullInfoById(Guid userId)
        {
            return await _userService.GetUserFullInfoAsync(userId);
        }


        [HttpGet]
        public async Task<List<UserShortInfoModel>> GetCurrentUserFollowings()
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await GetFollowings(userId);
        }

        [HttpGet]
        public async Task<List<UserShortInfoModel>> GetFollowings(Guid userId)
        {
            return await _userService.GetFollowingsAsync(userId);
        }

        [HttpGet]
        public async Task<List<UserShortInfoModel>> GetCurrentUserFollowers()
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await GetFollowers(userId);
        }

        [HttpGet]
        public async Task<List<UserShortInfoModel>> GetFollowers(Guid userId)
        {
            return await _userService.GetFollowersAsync(userId);
        }

        [HttpGet]
        public async Task<bool> CheckFollowing(Guid toWhomId)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await _userService.CheckFollowingAsync(userId, toWhomId);
        }

        [HttpPost]
        public async Task<bool> ChangeFollowing(Guid toWhomId)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            return await _userService.ChangeFollowingAsync(userId, toWhomId);
        }

    }
}
