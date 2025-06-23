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
        [Url(ErrorMessage = "URLError2")]
        [RegularExpression(@".+\.(jpeg|jpg|gif|png|webp|bmp|svg)$",
        ErrorMessage = "URLError")]
        public string? ImagePath { get; set; }

        public bool IsFavorite { get; set; } = false;

        public bool IsLastWord { get; set; } = false;

        public int SetId { get; set; }
    }
}