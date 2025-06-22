using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class PasswordViewModel
    {
        [Required(ErrorMessage = "RequiredMessage")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "RequiredMessage")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "StringLengthMessage")]
        [DataType(DataType.Password)]
        [Compare("ConfirmNewPassword", ErrorMessage = "ErrorForPassword")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "RequiredMessage")]
        [DataType(DataType.Password)]
        [DisplayName("ConfirmPassword")]
        public string? ConfirmNewPassword { get; set; }
    }
}