namespace Flashcards.Web.Areas.Sets.Models
{
    public class CurrentWordViewModel
    {
        public int Index { get; set; }
        public int Count { get; set; }
        public int SetId { get; set; }
        public WordViewModel CurrentWord { get; set; } = new();
    }
}
