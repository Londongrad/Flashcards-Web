namespace Flashcards.Web.Areas.Sets.Models
{
    public class CurrentWordViewModel
    {
        public int Count { get; set; }
        public WordViewModel CurrentWord { get; set; } = new();
    }
}