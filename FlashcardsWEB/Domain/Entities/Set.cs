using System.ComponentModel.DataAnnotations;

namespace FlashcardsWEB.Domain.Entities
{
    public class Set(int id, string name) : EntityBase(id)
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = name;
        
        public ICollection<Word>? Words { get; set; }
    }
}