using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;

namespace Flashcards.Infrastructure.Data
{
    public class DataManager(IRepository<Set> setRepository, IRepository<Word> wordRepository)
    {
        public IRepository<Set> SetRepository { get; set; } = setRepository;
        public IRepository<Word> WordRepository { get; set; } = wordRepository;
    }
}