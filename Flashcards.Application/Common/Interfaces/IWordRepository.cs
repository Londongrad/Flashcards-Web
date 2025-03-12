using Flashcards.Domain.Entities;

namespace Flashcards.Application.Common.Interfaces
{
    public interface IWordRepository
    {
        Task UpdateAsync(Word word);

        Task DeleteAsync(int id);

        Task<IEnumerable<Word>> GetAllAsync();

        Task<Word?> GetAsync(int id);

        Task AddAsync(Word word);
    }
}