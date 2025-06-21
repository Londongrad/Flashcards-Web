namespace Flashcards.Web.Areas.Sets.Models
{
    public class SetSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool HasImages { get; set; }
        public int WordCount { get; set; }
    }
}
