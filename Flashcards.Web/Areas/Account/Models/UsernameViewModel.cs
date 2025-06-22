using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class UsernameViewModel
    {
        [StringLength(10, MinimumLength = 4, ErrorMessage = "StringLengthMessage")]
        [Required(ErrorMessage = "RequiredMessage")]
        [Display (Name = "Username")]
        public string? Username { get; set; }
    }
}
