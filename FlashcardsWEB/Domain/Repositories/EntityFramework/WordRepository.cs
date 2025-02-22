using FlashcardsWEB.Domain.Data;
using FlashcardsWEB.Domain.Entities;
using FlashcardsWEB.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsWEB.Domain.Repositories.EntityFramework
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

        public Task<IEnumerable<Word>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Word?> GetAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}