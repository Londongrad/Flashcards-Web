using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Services;
using FluentAssertions;

namespace Flashcards.Tests
{
    public class SetStorageTests
    {
        [Fact]
        public void AddSetForUserThenReceiveItThenModify()
        {
            var storage1 = CreateStorage("user1");
            var storage2 = CreateStorage("user2");
            
            storage1.Set(new Set(1, "Set 1", "user1"));
            var result1 = storage1.Get();
            result1.Should().NotBeNull();
            result1?.Name.Should().Be("Set 1");

            storage2.Set(new Set(3, "Set 3", "user2"));
            var result2 = storage2.Get();
            result2.Should().NotBeNull();
            result2?.Name.Should().Be("Set 3");
        }

        private static SetStorage CreateStorage(string userId)
        {
            return new SetStorage(DatabaseTests.MockHttpContextAccessor(userId));
        }
    }
}
