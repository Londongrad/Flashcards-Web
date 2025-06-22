using Flashcards.Application.Interfaces;

namespace Flashcards.Web.Services
{
    public class DataManager(ISetRepository setRepository, IWordRepository wordRepository)
    {
        public ISetRepository SetRepository { get; } = setRepository;
        public IWordRepository WordRepository { get; } = wordRepository;
    }
}