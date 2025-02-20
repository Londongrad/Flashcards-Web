using System.ComponentModel.DataAnnotations;

namespace FlashcardsWEB.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Login is required.")]
        [Display(Name = "Login")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}