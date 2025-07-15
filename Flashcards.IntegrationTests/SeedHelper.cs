using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;

namespace Flashcards.IntegrationTests
{
    internal class SeedHelper
    {
        internal static readonly string user1_Id = "user1";
        internal static readonly string user2_Id = "user2";
        internal static void SeedData(ApplicationDbContext context)
        {
            if (context.Sets.Any())
                return;

            context.Sets.AddRange(
                new Set(1, "Set 1", user1_Id),

                new Set(2, "Set 2", user1_Id),

                new Set(3, "Set 3", user2_Id)
            );

            context.Words.AddRange(
                new Word(1, "Word 1", "Definition 1", 1),
                new Word(4, "Word 4", "Definition 4", 1),

                new Word(2, "Word 2", "Definition 2", 2),
                new Word(5, "Word 5", "Definition 5", 2),

                new Word(3, "Word 3", "Definition 3", 3),
                new Word(6, "Word 6", "Definition 6", 3)
            );

            context.SaveChanges();
        }
    }
}
