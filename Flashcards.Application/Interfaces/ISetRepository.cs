using Flashcards.Application.DTOs;
using Flashcards.Domain.Entities;

namespace Flashcards.Application.Interfaces
{
    public interface ISetRepository
    {
        Task<bool> IsNotUnique(string name, int id, string userId);
        Task UpdateAsync(Set set);
        Task AddAsync(Set set);
        Task DeleteAsync(int id, string userId);
        Task<Set?> GetAsync(int id, string userId);
        Task<Set?> GetWithWordsAsync(int id, string userId);
        Task<IEnumerable<SetDTO>> GetAllSummariesAsync(string userId);
        Task DeleteAllAsync(string userId);
        IQueryable<Set> GetUserSets(string userId);
    }
}
