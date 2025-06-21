using Flashcards.Domain.Entities;

namespace Flashcards.Application.Interfaces
{
    public interface IWordRepository
    {
        Task UpdateAsync(Word word);

        Task DeleteAsync(int id);

        Task<Word?> GetAsync(int id);

        Task AddAsync(Word word);

        Task<bool> IsNotUnique(string name, int id, string userId);
    }
}
