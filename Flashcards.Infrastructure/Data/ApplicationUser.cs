using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Flashcards.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Image URL")]
        public string? ImageURL { get; set; }
        public string? SessionToken { get; set; }
    }
}