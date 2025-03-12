using Flashcards.Domain.Entities;

namespace Flashcards.Application.Common.Interfaces
{
    public interface ISetRepository
    {
        Task UpdateAsync(Set set);

        Task DeleteAsync(int id);

        Task<IEnumerable<Set>> GetAllAsync(string userId);

        Task<Set?> GetAsync(int id);

        Task AddAsync(Set set);
    }
}