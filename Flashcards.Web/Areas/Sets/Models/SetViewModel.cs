using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Sets.Models
{
    public class SetViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name of the set")]
        [MaxLength(2, ErrorMessage = "The name of the set must be with a maximum length of 2 characters")]
        //[Remote(action: "CheckSet", controller: "Home", areaName: "Sets", HttpMethod = "POST", AdditionalFields = nameof(Id), ErrorMessage = "Set with this name already exists")]
        public string Name { get; set; } = null!;

        /// <summary>This property exists in order to avoid unnecessary calls to DB, when trying to rename set with the same value</summary>
        public string? OldName { get; set; }

        public List<WordViewModel> Words { get; set; } = [];

        public string? UserId { get; set; }
    }
}