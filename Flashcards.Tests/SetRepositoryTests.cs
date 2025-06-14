using System.Security.Claims;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Flashcards.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Flashcards.Tests;

public class SetRepositoryTests
{
    private static ApplicationDbContext CreateSqliteDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new ApplicationDbContext(options);
        context.Database.EnsureCreated();

        return context;
    }

    private static IHttpContextAccessor MockHttpContextAccessor(string userId)
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, userId) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var principal = new ClaimsPrincipal(identity);

        var context = new DefaultHttpContext { User = principal };
        var accessor = new Mock<IHttpContextAccessor>();
        accessor.Setup(x => x.HttpContext).Returns(context);

        return accessor.Object;
    }

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUsersSets()
    {
        var repo1 = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user1"));
        var repo2 = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user2"));

        await repo1.AddAsync(new Set(1, "Set 1", "user1"));
        await repo2.AddAsync(new Set(2, "Set 2", "user2"));

        var result = await repo1.GetAllAsync();

        result.Should().ContainSingle().Which.Name.Should().Be("Set 1");
    }

    [Fact]
    public async Task GetAsync_ReturnsSetIfBelongsToUser()
    {
        var repo1 = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user1"));
        var repo2 = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user2"));

        await repo1.AddAsync(new Set(1, "Set 1", "user1"));
        await repo2.AddAsync(new Set(2, "Set 2", "user2"));

        var result = await repo1.GetAsync(1);

        result.Should().NotBeNull();
        result!.Name.Should().Be("Set 1");
    }

    [Fact]
    public async Task AddAsync_AddsSetWithUserId()
    {
        var repo = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user1"));

        var set = new Set(1, "New Set", "user1");

        await repo.AddAsync(set);

        var saved = await repo.GetAsync(1);
        saved.Should().NotBeNull();
        saved.Name.Should().Be("New Set");
    }

    [Fact]
    public async Task DeleteAsync_RemovesUserSet()
    {
        var repo = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user1"));

        await repo.AddAsync(new Set(1, "Set", "user1"));

        await repo.DeleteAsync(1);

        var remaining = await repo.GetAsync(1);
        remaining.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ChangesName()
    {
        var context = CreateSqliteDbContext();
        var repo = new SetRepository(context, MockHttpContextAccessor("user1"));

        var originalSet = new Set(1, "Old Name", "user1");
        var updatedSet = new Set(1, "New Name", "user1");

        await repo.AddAsync(originalSet);
        await repo.UpdateAsync(updatedSet);

        var result = await repo.GetAsync(1);
        result!.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task IsNotUnique_ReturnsTrueIfNameExists()
    {
        var repo = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user1"));

        await repo.AddAsync(new Set(1, "Set", "user1"));

        var result = await repo.IsNotUnique("Set", 0);

        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAllAsync_RemovesAllUsersSets()
    {
        var repo1 = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user1"));
        var repo2 = new SetRepository(CreateSqliteDbContext(), MockHttpContextAccessor("user2"));

        await repo1.AddAsync(new Set(1, "Set 1", "user1"));
        await repo1.AddAsync(new Set(2, "Set 2", "user1"));
        await repo2.AddAsync(new Set(3, "Set 3", "user2"));

        await repo1.DeleteAllAsync();

        var remaining = await repo1.GetAllAsync();
        remaining.Should().BeEmpty();
    }
}
