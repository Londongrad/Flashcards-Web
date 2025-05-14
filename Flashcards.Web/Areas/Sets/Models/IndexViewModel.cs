namespace Flashcards.Web.Areas.Sets.Models
{
    public class IndexViewModel
    {
        public IEnumerable<SetViewModel> Sets { get; set; }

        public SetViewModel NewSet { get; set; }

        public IndexViewModel()
        {
            Sets = [];
            NewSet = new();
        }
    }
}
