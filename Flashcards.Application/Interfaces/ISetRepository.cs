using Flashcards.Application.DTOs;
using Flashcards.Domain.Entities;

namespace Flashcards.Application.Interfaces
{
    public interface ISetRepository : IRepository<Set>
    {
        Task<IEnumerable<SetDTO>> GetAllSummariesAsync();
        Task DeleteAllAsync();
    }
}
