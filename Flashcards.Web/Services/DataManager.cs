using Flashcards.Application.Interfaces;

namespace Flashcards.Web.Services
{
    public class DataManager(ISetRepository setRepository, IWordRepository wordRepository)
    {
        public ISetRepository SetRepository { get; set; } = setRepository;
        public IWordRepository WordRepository { get; set; } = wordRepository;
    }
}