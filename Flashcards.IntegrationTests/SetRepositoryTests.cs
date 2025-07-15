using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Repositories;
using Flashcards.IntegrationTests.Base;
using FluentAssertions;

namespace Flashcards.IntegrationTests;

public class SetRepositoryTests : SqliteIntegrationTestBase
{
    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUsersSets()
    {
        #region [ Arrange ]

        var repo = new SetRepository(Context);
        var user1Id = "user1";
        var user2Id = "user2";
        var set1 = new Set(1, "Set 1", user1Id);
        var set2 = new Set(2, "Set 2", user2Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        var result = await repo.GetAllSummariesAsync(user2Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().ContainSingle().Which.Name.Should().Be("Set 2");

        #endregion [ Assert ]
    }

    [Fact]
    public async Task GetAsync_ReturnsSetIfBelongsToUser()
    {
        #region [ Arrange ]

        var repo = new SetRepository(Context);
        var user1Id = "user1";
        var user2Id = "user2";
        var set1 = new Set(1, "Set 1", user1Id);
        var set2 = new Set(2, "Set 2", user2Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        var result = await repo.GetAsync(1, user1Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().NotBeNull();
        result.Name.Should().Be("Set 1");

        #endregion [ Assert ]
    }

    [Fact]
    public async Task DeleteAsync_RemovesUserSet()
    {
        #region [ Arrange ]

        var repo = new SetRepository(Context);
        var userId = "user1";
        var set1 = new Set(1, "Set 1", userId);
        var set2 = new Set(2, "Set 2", userId);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        await repo.DeleteAsync(1, userId);
        var remaining = await repo.GetAsync(2, userId);

        #endregion [ Act ]

        #region [ Assert ]

        remaining.Should().NotBeNull();
        remaining.Name.Should().Be("Set 2");

        #endregion [ Assert ]
    }

    [Fact]
    public async Task UpdateAsync_ChangesName()
    {
        #region [ Arrange ]

        var repo = new SetRepository(Context);
        var userId = "user1";
        var originalSet = new Set(1, "Old Name", userId);
        var updatedSet = new Set(1, "New Name", userId);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(originalSet);
        await repo.UpdateAsync(updatedSet);
        var result = await repo.GetAsync(1, userId);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().NotBeNull();
        result.Name.Should().Be("New Name");

        #endregion [ Assert ]
    }

    [Fact]
    public async Task IsNotUnique_ReturnsTrueIfNameExists_AddAction()
    {
        #region [ Arrange ]

        var repo = new SetRepository(Context);
        var userId = "user1";
        var set = new Set(1, "Set", userId);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set);
        var result = await repo.IsNotUnique("Set", 0, userId);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().BeTrue();

        #endregion [ Assert ]
    }

    [Fact]
    public async Task IsNotUnique_ReturnsTrueIfDuplicateExists_UpdateAction()
    {
        #region [ Arrange ]

        var repo = new SetRepository(Context);
        var userId = "user1";
        var set1 = new Set(1, "Set", userId);
        var set2 = new Set(2, "Set", userId);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        var result = await repo.IsNotUnique("Set", 1, userId);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().BeTrue();

        #endregion [ Assert ]
    }

    [Fact]
    public async Task DeleteAllAsync_RemovesAllUsersSets()
    {
        #region [ Arrange ]

        var repo = new SetRepository(Context);
        var userId = "user1";
        var set1 = new Set(1, "Set 1", userId);
        var set2 = new Set(2, "Set 2", userId);
        var set3 = new Set(3, "Set 3", userId);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        await repo.AddAsync(set3);
        await repo.DeleteAllAsync(userId);
        var remaining = await repo.GetAllSummariesAsync(userId);

        #endregion [ Act ]

        #region [ Assert ]

        remaining.Should().BeEmpty();

        #endregion [ Assert ]
    }

    [Fact]
    public async Task GetFavoriteAsync_ReturnsNullIfNotFavorite()
    {
        #region [ Arrange ]

        var userId = "user1";
        SeedHelper.SeedData(Context);

        var repo = new SetRepository(Context);

        #endregion [ Arrange ]

        #region [ Act ]

        var result = await repo.GetFavoriteAsync(1, userId);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().NotBeNull();
        result.Words.Should().BeEmpty();

        #endregion [ Assert ]
    }

    [Fact]
    public async Task GetWithWordsAsync_ReturnsSetWithItsWordsIfAny()
    {
        #region [ Arrange ]

        var userId = "user1";
        SeedHelper.SeedData(Context);

        var repo = new SetRepository(Context);

        List<Word> expectedWords = [
            new Word(1, "Word 1", "Definition 1", "", 1),
            new Word(4, "Word 4", "Definition 4", "", 1)
        ];

        #endregion [ Arrange ]

        #region [ Act ]

        var result = await repo.GetWithWordsAsync(1, userId);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().NotBeNull();
        result.Words.Should().BeEquivalentTo(expectedWords,
            options => options.Excluding(w => w.Set));

        #endregion [ Assert ]
    }
}
