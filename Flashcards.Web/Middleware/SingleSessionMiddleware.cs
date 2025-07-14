using Flashcards.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Flashcards.Web.Middleware
{
    public class SingleSessionMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();

            // Пути, которые НЕ должны проверяться
            var excludedPaths = new[]
            {
                "/account/login",
                "/account/logout",
                "/account/register",
                "/account/verifyemail",
                "/account/changepassword",
                "/css",
                "/js",
                "/lib"
            };

            if (path != null && excludedPaths.Any(path.StartsWith))
            {
                await _next(context);
                return;
            }

            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tokenFromClaims = context.User.FindFirst("SessionToken")?.Value;

                if (userId != null)
                {
                    var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var user = await userManager.FindByIdAsync(userId);

                    if (user != null && user.SessionToken != tokenFromClaims)
                    {
                        // Принудительный выход и удаление куки
                        await context.SignOutAsync(IdentityConstants.ApplicationScheme);
                        context.Response.Cookies.Delete(".AspNetCore.Identity.Application");

                        // Если это AJAX-запрос — просто 401
                        if (context.Request.Headers.XRequestedWith == "XMLHttpRequest")
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            return;
                        }

                        // Иначе редиректим на логин с флагом
                        context.Response.Redirect("/Account/Account/Login?forcedLogout=true");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
