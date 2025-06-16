using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Flashcards.Infrastructure.Repositories;
using FluentAssertions;

namespace Flashcards.Tests;

public class WordRepositoryTests
{
    [Fact]
    public async Task AddAsync_ReturnsWord()
    {
        var repo = CreateWordRep("user1");
        await repo.AddAsync(new Word(1, "Word 1", "Definition 1", "", 999));

        var result = await repo.GetAsync(1);
        result.Should().NotBeNull();
        result?.Name.Should().Be("Word 1");
    }

    [Fact]
    public async Task DeleteAsync_RemovesWord()
    {
        var repo = CreateWordRep("user1");
        await repo.AddAsync(new Word(1, "Word 1", "Definition 1", "", 999));

        await repo.DeleteAsync(1);
        var result = await repo.GetAsync(1);
        result.Should().BeNull();
    }

    [Fact]
    public async Task UpdateAsync_ChangesName()
    {
        var repo = CreateWordRep("user1");

        var word1 = new Word(1, "Word", "Definition 1", "", 999);
        var word2 = new Word(1, "New Word", "Definition 2", "", 999);

        await repo.AddAsync(word1);
        await repo.UpdateAsync(word2);

        var result = await repo.GetAsync(1);
        result!.Name.Should().Be("New Word");
    }

    [Fact]
    public async Task IsNotUnique_ReturnsTrueIfNameExists()
    {
        var repo = CreateWordRep("user1");

        await repo.AddAsync(new Word(1, "Word 1", "Definition 1", "", 999));
        await repo.AddAsync(new Word(2, "Word 2", "Definition 2", "", 999));

        var result1 = await repo.IsNotUnique("Word 1", 0);
        result1.Should().BeFalse();

        var result2 = await repo.IsNotUnique("Word 2", 2);
        result2.Should().BeFalse();
    }

    private static WordRepository CreateWordRep(string userId)
    {
        return new WordRepository(DatabaseTests.CreateSqliteDbContext(), DatabaseTests.MockHttpContextAccessor(userId));
    }
}
