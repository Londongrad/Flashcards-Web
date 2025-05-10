using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class UserViewModel
    {
        public string Id { get; set; } = null!;

        [Display(Name = "Image URL")]
        public string? ImageURL { get; set; }

        #region [ PASSWORD ]

        [Display(Name = "Old password")]
        [Required(ErrorMessage = "Old password is required")]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; } = null!;

        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [DataType(DataType.Password)]
        [DisplayName("New password")]
        [Required(ErrorMessage = "New password is required")]
        [Compare("ConfirmPassword", ErrorMessage = "Password does not match.")]
        public string? NewPassword { get; set; } = null!;

        [Display(Name = "Confirm new password")]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; } = null!;

        #endregion [ PASSWORD ]

        [StringLength(20, MinimumLength = 6, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }

        [StringLength(10, MinimumLength = 6, ErrorMessage = "The {0} must be at {2} and at max {1} characters long.")]
        [Required(ErrorMessage = "Username is required")]
        public string? Username { get; set; }
    }
}