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
        var set1 = new Set(1, "Set 1", SeedHelper.user1_Id);
        var set2 = new Set(2, "Set 2", SeedHelper.user2_Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        var result = await repo.GetAllSummariesAsync(SeedHelper.user2_Id);

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
        var set1 = new Set(1, "Set 1", SeedHelper.user1_Id);
        var set2 = new Set(2, "Set 2", SeedHelper.user2_Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        var result = await repo.GetAsync(1, SeedHelper.user1_Id);

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
        var set1 = new Set(1, "Set 1", SeedHelper.user1_Id);
        var set2 = new Set(2, "Set 2", SeedHelper.user1_Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        await repo.DeleteAsync(1, SeedHelper.user1_Id);
        var remaining = await repo.GetAsync(2, SeedHelper.user1_Id);

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
        var originalSet = new Set(1, "Old Name", SeedHelper.user1_Id);
        var updatedSet = new Set(1, "New Name", SeedHelper.user1_Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(originalSet);
        await repo.UpdateAsync(updatedSet);
        var result = await repo.GetAsync(1, SeedHelper.user1_Id);

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
        var set = new Set(1, "Set", SeedHelper.user1_Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set);
        var result = await repo.IsNotUnique("Set", 0, SeedHelper.user1_Id);

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
        var set1 = new Set(1, "Set", SeedHelper.user1_Id);
        var set2 = new Set(2, "Set", SeedHelper.user1_Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        var result = await repo.IsNotUnique("Set", 1, SeedHelper.user1_Id);

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
        var set1 = new Set(1, "Set 1", SeedHelper.user1_Id);
        var set2 = new Set(2, "Set 2", SeedHelper.user1_Id);
        var set3 = new Set(3, "Set 3", SeedHelper.user1_Id);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(set1);
        await repo.AddAsync(set2);
        await repo.AddAsync(set3);
        await repo.DeleteAllAsync(SeedHelper.user1_Id);
        var remaining = await repo.GetAllSummariesAsync(SeedHelper.user1_Id);

        #endregion [ Act ]

        #region [ Assert ]

        remaining.Should().BeEmpty();

        #endregion [ Assert ]
    }

    [Fact]
    public async Task GetFavoriteAsync_ReturnsNullIfNotFavorite()
    {
        #region [ Arrange ]

        SeedHelper.SeedData(Context);

        var repo = new SetRepository(Context);

        #endregion [ Arrange ]

        #region [ Act ]

        var result = await repo.GetFavoriteAsync(1, SeedHelper.user1_Id);

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

        SeedHelper.SeedData(Context);

        var repo = new SetRepository(Context);

        List<Word> expectedWords = [
            new Word(1, "Word 1", "Definition 1", 1),
            new Word(4, "Word 4", "Definition 4", 1)
        ];

        #endregion [ Arrange ]

        #region [ Act ]

        var result = await repo.GetWithWordsAsync(1, SeedHelper.user1_Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().NotBeNull();
        result.Words.Should().BeEquivalentTo(expectedWords,
            options => options.Excluding(w => w.Set));

        #endregion [ Assert ]
    }
}
