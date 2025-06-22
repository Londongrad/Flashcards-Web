using Flashcards.Application.DTOs;
using Flashcards.Domain.Entities;

namespace Flashcards.Application.Interfaces
{
    public interface ISetRepository
    {
        /// <summary>Checks whether a set name is not unique. <br/>
        /// If id == 0: check if any set with the same name exists. <br/>
        /// If id != 0: check if another set with the same name exists (used during updates).</summary>
        Task<bool> IsNotUnique(string name, int id, string userId);

        /// <summary>Updates the name of a set if it belongs to the current user.</summary>
        Task UpdateAsync(Set set);

        /// <summary>Adds a new set for the current user.</summary>
        Task AddAsync(Set set);

        /// <summary>Deletes a set with the specified ID if it belongs to the current user.</summary>
        Task DeleteAsync(int id, string userId);

        /// <summary>Retrieves a specific set by ID if it belongs to the current user without associated words.</summary>
        Task<Set?> GetAsync(int id, string userId);

        /// <summary>Retrieves a specific set by ID with corresponding words if it belongs to the current user.</summary>
        Task<Set?> GetWithWordsAsync(int id, string userId);

        /// <summary>Retrieves all sets that belong to the current user without their associated words.</summary>
        Task<IEnumerable<SetDTO>> GetAllSummariesAsync(string userId);

        /// <summary>Deletes all sets belonging to the current user.</summary>
        Task DeleteAllAsync(string userId);
    }
}
