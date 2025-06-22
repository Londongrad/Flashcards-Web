using Flashcards.Domain.Entities;

namespace Flashcards.Application.Interfaces
{
    public interface IWordRepository
    {
        /// <summary> Updates properties of an existing word using an efficient bulk update. </summary>
        Task UpdateAsync(Word word);

        /// <summary> Deletes a word by its ID. <br/>
        /// Uses ExecuteDeleteAsync for efficient deletion without loading the entity. </summary>
        Task DeleteAsync(int id);

        /// <summary> Retrieves a specific word by ID without tracking. </summary>
        Task<Word?> GetAsync(int id, int setId, string userId);

        /// <summary> Adds a new word to the database and saves changes. </summary>
        Task AddAsync(Word word);

        /// <summary> Checks if a word with the given name already exists (excluding the one with the given ID).
        /// Used for uniqueness validation. </summary>
        Task<bool> IsNotUnique(string name, int id, string userId);
    }
}
