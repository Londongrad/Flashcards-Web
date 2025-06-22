using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class EmailViewModel
    {
        [StringLength(20, MinimumLength = 6, ErrorMessage = "StringLengthMessage")]
        [Required(ErrorMessage = "RequiredMessage")]
        public string? Email { get; set; }
    }
}