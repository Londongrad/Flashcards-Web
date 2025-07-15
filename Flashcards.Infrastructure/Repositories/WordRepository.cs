using Flashcards.Application.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Infrastructure.Repositories
{
    public class WordRepository(ApplicationDbContext dbContext) : IWordRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        public async Task DeleteAsync(int id)
        {
            await _dbContext.Words
                .Where(w => w.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task AddAsync(Word word)
        {
            await _dbContext.Words.AddAsync(word);
            await _dbContext.SaveChangesAsync();
        }

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

        public async Task<Word?> GetAsync(int id, int setId, string userId)
        {
            return await _dbContext.Words
                .AsNoTracking()
                .Include(w => w.Set)
                .Where(w => w.Id == id &&
                            w.SetId == setId &&
                            w.Set!.UserId == userId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsNotUnique(string name, int id, string userId)
        {
            var query = _dbContext.Words.Include(w => w.Set).Where(w => w.Set!.UserId == userId);

            if (id == 0)
            {
                // Check if any word has the same name (new word creation)
                return await query.AnyAsync(w => w.Name == name);
            }

            // Check if any other word has the same name (on update)
            return await query.AnyAsync(w => w.Name == name && w.Id != id);
        }
    }
}