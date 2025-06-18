using Flashcards.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace Flashcards.Tests
{
    internal static class DatabaseTests
    {
        internal static ApplicationDbContext CreateSqliteDbContext()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(connection)
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            //For tests purposes
            context.Sets.Add(new Domain.Entities.Set(999, "SetForTests", "user999"));
            context.Words.Add(new Domain.Entities.Word(999, "WordForTests", "Definition", "", 999));

            return context;
        }

        internal static IHttpContextAccessor MockHttpContextAccessor(string userId)
        {
            var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var principal = new ClaimsPrincipal(identity);

            var context = new DefaultHttpContext { User = principal };
            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(x => x.HttpContext).Returns(context);

            return accessor.Object;
        }
    }
}
