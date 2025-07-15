using Flashcards.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.IntegrationTests.Base
{
    public abstract class SqliteIntegrationTestBase : IDisposable
    {
        protected readonly SqliteConnection Connection;
        protected readonly ApplicationDbContext Context;

        protected SqliteIntegrationTestBase()
        {
            Connection = new SqliteConnection("DataSource=:memory:");
            Connection.Open();

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(Connection)
                .Options;

            Context = new ApplicationDbContext(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Dispose();
            Connection.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
