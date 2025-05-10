namespace Flashcards.Domain.Entities
{
    public class Word(int id, string name, string definition, string imagePath, int setId, bool isFavorite = false, bool isLastWord = false) : EntityBase(id, name)
    {

        public string Definition { get; set; } = definition;

        public string? ImagePath { get; set; } = imagePath;

        public bool IsFavorite { get; set; } = isFavorite;

        public bool IsLastWord { get; set; } = isLastWord;

        public int SetId { get; set; } = setId;

        public Set? Set { get; set; }
    }
}