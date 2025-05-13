using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace Flashcards.Web.Areas.Sets.Models
{
    public class WordViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [Display(Name = "Name of the word")]
        [Remote(action: "CheckWord", controller: "Home", areaName: "Sets", HttpMethod = "POST", AdditionalFields = "Id", ErrorMessage = "Word with this name already exists")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Definition is required.")]
        [Display(Name = "Definition of the word")]
        public string Definition { get; set; } = "";

        [Display(Name = "Image URL")]
        [ValidateNever]
        public string? ImagePath { get; set; }

        public bool IsFavorite { get; set; } = false;

        public bool IsLastWord { get; set; } = false;

        public int SetId { get; set; }
    }
}