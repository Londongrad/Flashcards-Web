using Flashcards.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace FlashcardsWEB.ViewModels
{
    public class NewSetViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name of the set")]
        [MaxLength(2, ErrorMessage = "The name of the set must with a maximum length of 2 characters")]
        public string? Name { get; set; }

        public List<Word>? Words { get; set; }
        public DateOnly TimeCreated { get; set; }
    }
}