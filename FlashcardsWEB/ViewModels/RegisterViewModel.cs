using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FlashcardsWEB.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Login is required.")]
        [Display(Name = "Login")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [DisplayName("Confirm password")]
        public string? ConfirmPassword { get; set; }
    }
}