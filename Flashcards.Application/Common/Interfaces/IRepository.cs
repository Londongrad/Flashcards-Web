using Flashcards.Domain.Entities;

namespace Flashcards.Application.Common.Interfaces
{
    public interface IRepository<T> where T : EntityBase
    {
        Task UpdateAsync(T entity);

        Task DeleteAsync(int id);

        Task<IEnumerable<T>> GetAllAsync();

        Task<T?> GetAsync(int id);
    }
}