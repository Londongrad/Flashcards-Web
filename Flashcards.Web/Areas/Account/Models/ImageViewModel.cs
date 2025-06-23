using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Account.Models
{
    public class ImageViewModel
    {
        [Display(Name = "ImageURL")]
        [Url(ErrorMessage = "URLError2")]
        [RegularExpression(@".+\.(jpeg|jpg|gif|png|webp|bmp|svg)$",
        ErrorMessage = "URLError")]
        public string? ImageURL { get; set; }
    }
}
