using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Infrastructure.Repositories
{
    public class WordRepository(ApplicationDbContext dbContext) : IRepository<Word>
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        /// <summary> Deletes a word by its ID. <br/>
        /// Uses ExecuteDeleteAsync for efficient deletion without loading the entity. </summary>
        public async Task DeleteAsync(int id)
        {
            await _dbContext.Words
                .Where(w => w.Id == id)
                .ExecuteDeleteAsync();
        }

        /// <summary> Adds a new word to the database and saves changes. </summary>
        public async Task AddAsync(Word word)
        {
            await _dbContext.Words.AddAsync(word);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary> Updates properties of an existing word using an efficient bulk update. </summary>
        public async Task UpdateAsync(Word word)
        {
            await _dbContext.Words.Where(w => w.Id == word.Id)
                .ExecuteUpdateAsync(sp => sp
                   .SetProperty(c => c.Name, word.Name)
                   .SetProperty(c => c.Definition, word.Definition)
                   .SetProperty(c => c.ImagePath, word.ImagePath)
                   .SetProperty(c => c.IsFavorite, word.IsFavorite)
                );
        }

        /// <summary> Retrieves all words without tracking (read-only). </summary>
        public async Task<IEnumerable<Word>> GetAllAsync()
        {
            return await _dbContext.Words.AsNoTracking().ToListAsync();
        }

        /// <summary> Retrieves a specific word by ID without tracking. </summary>
        public async Task<Word?> GetAsync(int id)
        {
            return await _dbContext.Words.AsNoTracking().FirstOrDefaultAsync(w => w.Id == id);
        }

        /// <summary> Checks if a word with the given name already exists (excluding the one with the given ID).
        /// Used for uniqueness validation. </summary>
        public async Task<bool> IsNotUnique(string name, int id)
        {
            var query = _dbContext.Words.AsQueryable();

            if (id == 0)
            {
                // Check if any word has the same name (new word creation)
                return await query.AnyAsync(w => w.Name == name);
            }

            // Check if any other word has the same name (on update)
            return await query.AnyAsync(w => w.Name == name && w.Id != id);
        }

        /// <summary>
        /// Deletes all words (not implemented).
        /// </summary>
        public Task DeleteAllAsync() => throw new NotImplementedException();
    }
}