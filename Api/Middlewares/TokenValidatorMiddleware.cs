using Api.Services;
using Common.Consts;
using Common.Extensions;

namespace Api.Middlewares
{
    public class TokenValidatorMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenValidatorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, CheckService checkService /*AuthService authService*/)
        {
            var isOk = true;
            
            if (context.User.TryFindGuidValue(CustomClaimNames.SessionId, out var sessionId))
            {
                //var session = await authService.GetSessionByIdAsync(sessionId);
                var session = await checkService.FindSessionOrThrowExAsync(sessionId);

                if (!session.IsActive)
                {
                    isOk = false;
                    await Results.Unauthorized().ExecuteAsync(context);
                }
            }

            if (isOk)
            {
                await _next(context);
            }
        }
    }
    public static class TokenValidatorMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenValidator(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenValidatorMiddleware>();
        }
    }
}
