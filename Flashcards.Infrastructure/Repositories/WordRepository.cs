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

        public async Task UpdateAsync(Word entity)
        {
            dbContext.Entry(entity).State = entity.Id == default ? EntityState.Added : EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Word>> GetAllAsync()
        {
            return await dbContext.Words.ToListAsync();
        }

        public async Task<Word?> GetAsync(int id)
        {
            return await dbContext.Words.FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}