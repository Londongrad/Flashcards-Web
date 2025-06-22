using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Flashcards.Web.Areas.Account.Models
{
    public class PasswordInSettingsViewModel
    {
        [Display(Name = "OldPassword")]
        [Required(ErrorMessage = "RequiredMessage")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [StringLength(20, MinimumLength = 6, ErrorMessage = "StringLengthMessage")]
        [DataType(DataType.Password)]
        [DisplayName("NewPassword")]
        [Required(ErrorMessage = "RequiredMessage")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match")]
        public string? NewPassword { get; set; }

        [Display(Name = "ConfirmPassword")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "RequiredMessage")]
        public string? ConfirmPassword { get; set; }
    }
}
