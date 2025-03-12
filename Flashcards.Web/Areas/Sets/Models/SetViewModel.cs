using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Sets.Models
{
    public class SetViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name of the set")]
        [MaxLength(2, ErrorMessage = "The name of the set must be with a maximum length of 2 characters")]
        [Remote(action: "CheckSet", controller: "Home", areaName: "Sets", ErrorMessage = "Set with this name is already exists")]
        public string Name { get; set; } = null!;

        public List<WordViewModel> Words { get; set; } = [];
    }
}