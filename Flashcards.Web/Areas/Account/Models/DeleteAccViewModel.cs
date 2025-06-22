using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class DeleteAccViewModel
    {
        [StringLength(20, MinimumLength = 6, ErrorMessage = "StringLengthMessage")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "RequiredMessage")]
        [Display (Name = "Password")]
        public string? Password { get; set; }
    }
}
