using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "RequiredMessage")]
        [Display(Name = "Login")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "RequiredMessage")]
        [DataType(DataType.Password)]
        [Display (Name = "Password")]
        public string? Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }
    }
}