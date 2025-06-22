using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class ImageViewModel
    {
        [Display(Name = "ImageURL")]
        [ValidateNever]
        public string? ImageURL { get; set; }
    }
}
