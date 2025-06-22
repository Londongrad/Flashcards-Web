using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Sets.Models
{
    public class WordViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredMessage")]
        [Display(Name = "WordName")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "RequiredMessage")]
        [Display(Name = "WordDefinition")]
        public string Definition { get; set; } = "";

        [Display(Name = "ImageURL")]
        [ValidateNever]
        public string? ImagePath { get; set; }

        public bool IsFavorite { get; set; } = false;

        public bool IsLastWord { get; set; } = false;

        public int SetId { get; set; }
    }
}