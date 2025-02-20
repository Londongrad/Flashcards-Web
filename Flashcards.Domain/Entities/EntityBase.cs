namespace Flashcards.Domain.Entities
{
    public abstract class EntityBase(int id)
    {
        public int Id { get; set; } = id;

        public DateOnly TimeCreated { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);
    }
}