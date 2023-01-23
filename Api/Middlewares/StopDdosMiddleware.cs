using Api.Exceptions;
using Api.Services;

namespace Api.Middlewares
{
    public class StopDdosMiddleware
    {
        private readonly RequestDelegate _next;

        public StopDdosMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, DdosGuard guard)
        {
            var headerAuth = context.Request.Headers.Authorization;

            try
            {
                guard.CheckRequest(headerAuth);
                await _next(context);
            }
            catch (TooManyRequestsException ex)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsJsonAsync($"{ex.Message}. Only 10 requests per second are allowed");
            }
        }
    }

    public static class StopDdosMiddlewareMiddlewareExtensions
    {
        public static IApplicationBuilder UseAntiDdosCustom(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<StopDdosMiddleware>();
        }
    }
}