using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Repositories;
using Flashcards.IntegrationTests.Base;
using FluentAssertions;

namespace Flashcards.IntegrationTests;

public class WordRepositoryTests : SqliteIntegrationTestBase
{
    [Fact]
    public async Task AddAsync_ReturnsWord()
    {
        #region [ Arrange ]

        SeedHelper.SeedData(Context);
        var repo = new WordRepository(Context);
        var word = new Word(7, "Word 7", "Definition 7", 3);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(word);
        var result = await repo.GetAsync(word.Id, word.SetId, SeedHelper.user2_Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().NotBeNull();
        result?.Name.Should().Be("Word 7");

        #endregion [ Assert ]
    }

    [Fact]
    public async Task DeleteAsync_RemovesWord()
    {
        #region [ Arrange ]

        SeedHelper.SeedData(Context);
        var repo = new WordRepository(Context);
        var word = new Word(6, "Word 6", "Definition 6", 3);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.DeleteAsync(6);
        var result = await repo.GetAsync(word.Id, word.SetId, SeedHelper.user2_Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().BeNull();

        #endregion [ Assert ]
    }

    [Fact]
    public async Task UpdateAsync_ChangesName()
    {
        #region [ Arrange ]

        SeedHelper.SeedData(Context);
        var repo = new WordRepository(Context);
        var newWord = new Word(1, "New Word", "Definition 2", 1);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.UpdateAsync(newWord);
        var result = await repo.GetAsync(newWord.Id, newWord.SetId, SeedHelper.user1_Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().NotBeNull();
        result.Name.Should().Be("New Word");
        result.Definition.Should().Be("Definition 2");

        #endregion [ Assert ]
    }

    [Fact]
    public async Task IsNotUnique_ReturnsTrueIfNameExists_AddAction()
    {
        #region [ Arrange ]

        SeedHelper.SeedData(Context);
        var repo = new WordRepository(Context);
        var newWord = new Word(1, "Word 1", "Definition 1", 1);

        #endregion [ Arrange ]

        #region [ Act ]

        var result = await repo.IsNotUnique(newWord.Name, 0, SeedHelper.user1_Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().BeTrue();

        #endregion [ Assert ]
    }

    [Fact]
    public async Task IsNotUnique_ReturnsTrueIfDuplicateExists_UpdateAction()
    {
        #region [ Arrange ]

        SeedHelper.SeedData(Context);
        var repo = new WordRepository(Context);
        var newWord = new Word(10, "Word 1", "Definition 1", 1);

        #endregion [ Arrange ]

        #region [ Act ]

        await repo.AddAsync(newWord);
        var result = await repo.IsNotUnique(newWord.Name, newWord.Id, SeedHelper.user1_Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().BeTrue();

        #endregion [ Assert ]
    }
}
