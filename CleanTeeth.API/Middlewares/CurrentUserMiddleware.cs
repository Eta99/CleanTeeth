using CleanTeeth.API.Services;
using CleanTeeth.Application.Contracts.Services;
using System.Security.Claims;

namespace CleanTeeth.API.Middlewares
{
    public class CurrentUserMiddleware
    {
        private readonly RequestDelegate _next;

        public CurrentUserMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IEnsureCurrentUserService ensureCurrentUserService)
        {
            var login =
                context.User.Identity?.Name ??
                context.User.FindFirstValue(ClaimTypes.Name) ??
                context.User.FindFirstValue(ClaimTypes.Upn);
            CurrentUserInfo? userInfo = null;

            if (context.User.Identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(login))
                userInfo = await ensureCurrentUserService.EnsureAsync(login, context.RequestAborted);

            context.Items[CurrentUserContext.CurrentUserKey] = userInfo;

            await _next(context);
        }
    }

    public static class CurrentUserMiddlewareExtensions
    {
        public static IApplicationBuilder UseCurrentUser(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CurrentUserMiddleware>();
        }
    }
}
