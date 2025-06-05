using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Flashcards.Web.Areas.Account.Models
{
    public class PasswordInSettingsViewModel
    {
        [Display(Name = "Old password")]
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        [Required(ErrorMessage = "New password is required")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public string? NewPassword { get; set; }

        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Confirm password is required")]
        public string? ConfirmPassword { get; set; }
    }
}
