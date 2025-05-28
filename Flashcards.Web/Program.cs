using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Flashcards.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Flashcards.Web
{
    public class Program
    {
        //TODO: Реализовать систему отслеживания прогресса по обучению
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>
                (
                    options => options.UseSqlServer(builder.Configuration.GetConnectionString("Host"))
                );

            //Identity system
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

            // Auth cookie
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "myAppAuth";
                options.Cookie.HttpOnly = true;
                options.LoginPath = "/account/account/login";
                options.SlidingExpiration = true;
            });

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IRepository<Set>, SetRepository>();
            builder.Services.AddScoped<IRepository<Word>, WordRepository>();
            builder.Services.AddScoped<DataManager>();
            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            var app = builder.Build();

            app.UseStaticFiles();

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{area=Sets}/{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}