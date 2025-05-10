namespace Flashcards.Domain.Entities
{
    public abstract class EntityBase(int id, string name)
    {
        public int Id { get; set; } = id;
        public string Name { get; set; } = name;
    }
}