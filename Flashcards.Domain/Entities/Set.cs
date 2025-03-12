namespace Flashcards.Domain.Entities
{
    public class Set(int id, string name, string userId) : EntityBase(id)
    {
        public string Name { get; set; } = name;

        public List<Word>? Words { get; set; }

        public string UserId { get; set; } = userId;
    }
}