using Flashcards.Application.Common.Interfaces;

namespace Flashcards.Infrastructure.Data
{
    public class DataManager(ISetRepository setRepository, IWordRepository wordRepository)
    {
        public ISetRepository SetRepository { get; set; } = setRepository;
        public IWordRepository WordRepository { get; set; } = wordRepository;
    }
}