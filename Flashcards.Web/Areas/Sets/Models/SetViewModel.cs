using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Sets.Models
{
    public class SetViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "RequiredMessage")]
        [Display(Name = "SetName")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "StringLengthMessage")]
        public string Name { get; set; } = null!;

        /// <summary>This property exists in order to avoid unnecessary calls to DB, when trying to rename set with the same value</summary>
        public string? OldName { get; set; }

        public List<WordViewModel> Words { get; set; } = [];

        public string? UserId { get; set; }

        public bool HasImages { get; set; }
    }
}