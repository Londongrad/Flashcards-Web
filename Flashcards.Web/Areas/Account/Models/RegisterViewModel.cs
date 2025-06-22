using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "RequiredMessage")]
        [Display(Name = "Login")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "RequiredMessage")]
        [EmailAddress]
        [Display (Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "RequiredMessage")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "StringLengthMessage")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [Compare("ConfirmPassword", ErrorMessage = "ErrorForPassword")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "RequiredMessage")]
        [DataType(DataType.Password)]
        [DisplayName("ConfirmPassword")]
        public string? ConfirmPassword { get; set; }
    }
}