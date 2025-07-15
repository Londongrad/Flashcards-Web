namespace Flashcards.UnitTests;

public class SetRepositoryTests
{
    [Fact]
    public async Task GetAsync_ReturnsNullIfSetDoesNotBelongToUser()
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
        var result = await repo.GetAsync(1, user2Id);

        #endregion [ Act ]

        #region [ Assert ]

        result.Should().BeNull();

        #endregion [ Assert ]

    }
}