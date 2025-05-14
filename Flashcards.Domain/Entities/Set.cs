namespace Flashcards.Domain.Entities
{
    public class Set(int id, string name, string userId) : EntityBase(id, name)
    {
        public List<Word> Words { get; set; } = [];

        public string UserId { get; set; } = userId;
    }
}