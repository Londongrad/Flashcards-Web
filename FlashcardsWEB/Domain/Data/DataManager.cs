using FlashcardsWEB.Domain.Entities;
using FlashcardsWEB.Domain.Repositories.Abstract;

namespace FlashcardsWEB.Domain.Data
{
    public class DataManager(IRepository<Set> setRepository, IRepository<Word> wordRepository)
    {
        public IRepository<Set> SetRepository { get; set; } = setRepository;
        public IRepository<Word> WordRepository { get; set; } = wordRepository;
    }
}