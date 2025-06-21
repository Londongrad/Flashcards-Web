using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Flashcards.Web.Common
{
    public abstract class BaseCotroller : Controller
    {
        protected string? GetUserId()
        {
            return User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
