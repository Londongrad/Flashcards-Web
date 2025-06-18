using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Services;
using FluentAssertions;

namespace Flashcards.Tests
{
    public class SetStorageTests
    {
        [Fact]
        public void AddSetForUserThenReceiveIt()
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

        [Fact]
        public void ModifyWord()
        {
            var storage = CreateStorage("user1");

            var set = new Set(1, "Set 1", "user1");
            var word = new Word(999, "Word", "Definition", "", 1);
            set.Words.Add(word);

            storage.Set(set);
            storage.Modify(new Word(999, "New Word", "Definition", "", 1));
            var result = storage.Get();

            result.Should().NotBeNull();
            result?.Words.FirstOrDefault(w => w.Id == word.Id)?.Name.Should().Be("New Word");
        }

        private static SetStorage CreateStorage(string userId)
        {
            return new SetStorage(DatabaseTests.MockHttpContextAccessor(userId));
        }
    }
}
