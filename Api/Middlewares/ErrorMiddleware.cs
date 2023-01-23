using Api.Exceptions;
using Api.Exceptions.ExistsExceptions;
using Api.Exceptions.IncorrectExceptions;
using Api.Exceptions.NotFoundExceptions;
using Microsoft.IdentityModel.Tokens;

namespace Api.Middlewares
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SecurityTokenException ex)
            {
                await Results.BadRequest(ex.Message).ExecuteAsync(context);
            }
            catch (UnauthorizedException)
            {
                await Results.Unauthorized().ExecuteAsync(context);
            }
            catch (NotFoundException ex)
            {
                await Results.NotFound(ex.Message).ExecuteAsync(context);
            }
            catch (ExistsException ex)
            {
                await Results.BadRequest(ex.Message).ExecuteAsync(context);
            }
            catch (IncorrectException ex)
            {
                await Results.BadRequest(ex.Message).ExecuteAsync(context);
            }
            catch (ItemException ex)
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                await context.Response.WriteAsync(ex.Message);
            }

        }
    }
    public static class ErrorMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalErrorWrapper(
            this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorMiddleware>();
        }
    }
}
