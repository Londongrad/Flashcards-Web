using Flashcards.Application.Interfaces;
using Flashcards.Infrastructure.Data;
using Flashcards.Infrastructure.Repositories;
using Flashcards.Web.Middleware;
using Flashcards.Web.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Flashcards.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>
                (
                    options => options.UseSqlServer(builder.Configuration.GetConnectionString("Host"))
                );

            // Add services to the container.
            // Identity system
            builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Auth login cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "myAppAuth";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/account/account/login";
                options.SlidingExpiration = true;
            });

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<ISetRepository, SetRepository>();
            builder.Services.AddScoped<IWordRepository, WordRepository>();
            builder.Services.AddScoped<DataManager>();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            var app = builder.Build();

            // Этот участок кода помог решить проблему, при которой от Ajax приходил http запрос вместо https
            var forwardedHeaderOptions = new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
                RequireHeaderSymmetry = false,
                ForwardLimit = null
            };
            forwardedHeaderOptions.KnownNetworks.Clear();
            forwardedHeaderOptions.KnownProxies.Clear();

            app.UseForwardedHeaders(forwardedHeaderOptions);

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<SingleSessionMiddleware>();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Sets}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}