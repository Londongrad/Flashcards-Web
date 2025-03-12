using Microsoft.AspNetCore.Identity;

namespace Flashcards.Infrastructure.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? ImageURL { get; set; }
    }
}
