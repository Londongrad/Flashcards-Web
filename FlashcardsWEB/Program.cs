using FlashcardsWEB.Domain.Data;
using FlashcardsWEB.Domain.Entities;
using FlashcardsWEB.Domain.Repositories.Abstract;
using FlashcardsWEB.Domain.Repositories.EntityFramework;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FlashcardsWEB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //Appsettings.json
            IConfigurationBuilder configBuild = new ConfigurationBuilder()
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();

            IConfiguration configuration = configBuild.Build();

            builder.Services.AddDbContext<ApplicationDbContext>
                (
                    //options => options.UseSqlServer(builder.Configuration.GetConnectionString("ASP_Project"))
                    options => options.UseSqlite("Data Source=asp_project.db")
                );

            //Identity system
            builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
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

            //Auth cookie
            //builder.Services.ConfigureApplicationCookie(options =>
            //{
            //    options.Cookie.Name = "myAppAuth";
            //    options.Cookie.HttpOnly = true;
            //    options.LoginPath = "/account/login";
            //    options.AccessDeniedPath = "/admin/accessdenied";
            //    options.SlidingExpiration = true;
            //});

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddTransient<IRepository<Set>, SetRepository>();
            builder.Services.AddTransient<IRepository<Word>, WordRepository>();
            builder.Services.AddTransient<DataManager>();
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
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}