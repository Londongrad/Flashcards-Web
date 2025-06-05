using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class EmailViewModel
    {
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
    }
}
