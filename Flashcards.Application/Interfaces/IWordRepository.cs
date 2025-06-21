using Flashcards.Domain.Entities;

namespace Flashcards.Application.Interfaces
{
    public interface IWordRepository : IRepository<Word>
    {
        Task<IEnumerable<Word>> GetAllAsync();
    }
}
