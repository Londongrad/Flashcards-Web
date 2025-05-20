using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Infrastructure.Repositories
{
    public class WordRepository(ApplicationDbContext dbContext) : IRepository<Word>
    {
        public async Task DeleteAsync(int id)
        {
            await dbContext.Words.Where(w => w.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task AddAsync(Word word)
        {
            await dbContext.Words.AddAsync(word);
            await dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Word word)
        {
            await dbContext.Words.Where(w => w.Id == word.Id)
                .ExecuteUpdateAsync(sp => sp
                   .SetProperty(c => c.Name, word.Name)
                   .SetProperty(c => c.Definition, word.Definition)
                   .SetProperty(c => c.ImagePath, word.ImagePath)
                   .SetProperty(c => c.IsFavorite, word.IsFavorite)
                );
        }

        public async Task<IEnumerable<Word>> GetAllAsync()
        {
            return await dbContext.Words.AsNoTracking().ToListAsync();
        }

        public async Task<Word?> GetAsync(int id)
        {
            return await dbContext.Words.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<bool> IsNotUnique(string name, int id) => await Task.Run(() =>
        {
            if (id == 0)
                return dbContext.Set<Word>().Any(w => w.Name == name);
            else
                return dbContext.Set<Word>().Any(w => w.Name == name && w.Id != id);
        });
    }
}