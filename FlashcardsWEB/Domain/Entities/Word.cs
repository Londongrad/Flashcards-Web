using System.ComponentModel.DataAnnotations;

namespace FlashcardsWEB.Domain.Entities
{
    public class Word(int id, string name, string definition, string imagePath, int setId, bool isFavorite = false, bool isLastWord = false) : EntityBase(id)
    {
        [MaxLength(30)]
        public string Name { get; set; } = name;

        [MaxLength(200)]
        public string Definition { get; set; } = definition;

        [MaxLength(300)]
        public string ImagePath { get; set; } = imagePath;

        public int SetId { get; set; } = setId;

        public bool IsFavorite { get; set; } = isFavorite;

        public bool IsLastWord { get; set; } = isLastWord;

        public Set Set { get; set; } = null!;
    }
}