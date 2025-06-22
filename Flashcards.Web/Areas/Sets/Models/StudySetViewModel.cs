namespace Flashcards.Web.Areas.Sets.Models
{
    public class StudySetViewModel
    {
        public int Count { get; set; }
        public WordViewModel FirstWord { get; set; } = null!;
        public string WordsJson { get; set; } = "";
    }
}