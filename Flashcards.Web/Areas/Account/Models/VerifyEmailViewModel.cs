using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "RequiredMessage")]
        [EmailAddress]
        [Display (Name = "Email")]
        public string? Email { get; set; }
    }
}