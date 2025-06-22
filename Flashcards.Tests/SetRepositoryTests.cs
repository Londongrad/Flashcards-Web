//using Flashcards.Domain.Entities;
//using Flashcards.Infrastructure.Data;
//using Flashcards.Infrastructure.Repositories;
//using FluentAssertions;

//namespace Flashcards.Tests;

//public class SetRepositoryTests
//{
//    [Fact]
//    public async Task GetAllAsync_ReturnsOnlyUsersSets()
//    {
//        var context = DatabaseTests.CreateSqliteDbContext();
//        var repo1 = CreateSetRep(context, "user1");
//        var repo2 = CreateSetRep(context, "user2");

//        await repo1.AddAsync(new Set(1, "Set 1", "user1"));
//        await repo2.AddAsync(new Set(2, "Set 2", "user2"));

//        var result = await repo1.GetAllAsync();

//        result.Should().ContainSingle().Which.Name.Should().Be("Set 1");
//    }

//    [Fact]
//    public async Task GetAsync_ReturnsSetIfBelongsToUser()
//    {
//        var context = DatabaseTests.CreateSqliteDbContext();
//        var repo1 = CreateSetRep(context, "user1");
//        var repo2 = CreateSetRep(context, "user2");

//        await repo1.AddAsync(new Set(1, "Set 1", "user1"));
//        await repo2.AddAsync(new Set(2, "Set 2", "user2"));

//        var result = await repo1.GetAsync(1);

//        result.Should().NotBeNull();
//        result!.Name.Should().Be("Set 1");
//    }

//    [Fact]
//    public async Task AddAsync_AddsSetWithUserId()
//    {
//        var repo = CreateSetRep("user1");

//        var set = new Set(1, "New Set", "user1");

//        await repo.AddAsync(set);

//        var saved = await repo.GetAsync(1);
//        saved.Should().NotBeNull();
//        saved.Name.Should().Be("New Set");
//    }

//    [Fact]
//    public async Task DeleteAsync_RemovesUserSet()
//    {
//        var repo = CreateSetRep("user1");

//        await repo.AddAsync(new Set(1, "Set", "user1"));

//        await repo.DeleteAsync(1);

//        var remaining = await repo.GetAsync(1);
//        remaining.Should().BeNull();
//    }

//    [Fact]
//    public async Task UpdateAsync_ChangesName()
//    {
//        var repo = CreateSetRep("user1");

//        var originalSet = new Set(1, "Old Name", "user1");
//        var updatedSet = new Set(1, "New Name", "user1");

//        await repo.AddAsync(originalSet);
//        await repo.UpdateAsync(updatedSet);

//        var result = await repo.GetAsync(1);
//        result!.Name.Should().Be("New Name");
//    }

//    [Fact]
//    public async Task IsNotUnique_ReturnsTrueIfNameExists()
//    {
//        var repo = CreateSetRep("user1");

//        await repo.AddAsync(new Set(1, "Set", "user1"));
//        var result = await repo.IsNotUnique("Set", 0);
//        result.Should().BeTrue();

//        var resultForUpdate = await repo.IsNotUnique("Set", 1);
//        resultForUpdate.Should().BeFalse();
//    }

//    [Fact]
//    public async Task DeleteAllAsync_RemovesAllUsersSets()
//    {
//        var context = DatabaseTests.CreateSqliteDbContext();

//        var repo1 = CreateSetRep(context, "user1");
//        var repo2 = CreateSetRep(context, "user2");

//        await repo1.AddAsync(new Set(1, "Set 1", "user1"));
//        await repo1.AddAsync(new Set(2, "Set 2", "user1"));
//        await repo2.AddAsync(new Set(3, "Set 3", "user2"));

//        await repo1.DeleteAllAsync();
//        var remaining = await repo1.GetAllAsync();
//        remaining.Should().BeEmpty();

//        var user2Sets = await repo2.GetAllAsync();
//        user2Sets.Should().NotBeNull();
//        user2Sets.Should().ContainSingle().Which.Name.Should().Be("Set 3");
//    }

//    internal static SetRepository CreateSetRep(ApplicationDbContext context, string userId)
//    {
//        return new SetRepository(context, DatabaseTests.MockHttpContextAccessor(userId));
//    }
//    internal static SetRepository CreateSetRep(string userId)
//    {
//        return new SetRepository(DatabaseTests.CreateSqliteDbContext(), DatabaseTests.MockHttpContextAccessor(userId));
//    }
//}
