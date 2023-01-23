
using Api.Models.Push;
using Api.Services;
using Common.Consts;
using DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    [ApiExplorerSettings(GroupName = "Api")]
    public class PushController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly GooglePushService _googlePushService;
        private readonly CheckService _checkService;
        
        public PushController(UserService userService, GooglePushService googlePushService, CheckService checkService)
        {
            _userService = userService;
            _googlePushService = googlePushService;
            _checkService = checkService;
        }

        [HttpPost]
        public async Task Subscribe(PushTokenModel model)
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            await _userService.SetPushTokenAsync(userId, model.Token);
        }

        [HttpDelete]
        public async Task Unsubscribe()
        {
            var userId = _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            await _userService.SetPushTokenAsync(userId);
        }

        [HttpPost]
        public async Task<List<string>> SendPush(SendPushModel model)
        {
            var res = new List<string>();
            var userId = model.UserId ?? _checkService.GetGuidFromClaimsOrThrowEx(User, CustomClaimNames.Id);
            var token = await _userService.GetPushTokenAsync(userId);
            
            if (token != default)
                res = _googlePushService.SendNotification(token, model.Push);

            return res;
        }
    }
}