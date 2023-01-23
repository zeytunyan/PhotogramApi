using Api.Exceptions;
using Api.Models.Token;
using Api.Models.User;
using Api.Services;
using Common;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Auth")]
    public class AuthController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly AuthService _authService;
        private readonly UserService _userService;
        private readonly CheckService _checkService;

        public AuthController(
            AuthService authService, 
            UserService userService, 
            EmailService emailService,
            CheckService checkService)
        {
            _authService = authService;
            _userService = userService;
            _emailService = emailService;
            _checkService = checkService;
        }

        [HttpPost]
        public async Task<UserPageInfoModel> RegisterUser(UserRegistrationModel model)
        {
            await _checkService.ThrowExIfUserExistsAsync(model.Email, model.PhoneNumber, model.NickName);

            var createdUser = await _userService.CreateUserAsync(model);
            await _emailService.SendWelcomeEmailAsync(model.Email, model.NickName);
            return createdUser;
        }

        [HttpPost]
        public async Task<TokenModel> Token(TokenRequestModel model)
            => await _authService.GetTokenAsync(model.Login, model.Pass);

        [HttpPost]
        public async Task<TokenModel> RefreshToken(RefreshTokenRequestModel model)
            => await _authService.GetTokenByRefreshTokenAsync(model.RefreshToken);

    }
}
