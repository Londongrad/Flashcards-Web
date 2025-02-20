using FlashcardsWEB.Domain.Entities;
using FlashcardsWEB.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsWEB.Domain.Repositories.EntityFramework
{
    public class WordRepository : IRepository<Word>
    {
        private readonly ApplicationDbContext _dbContext;

        public WordRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAsync(int id)
        {
            await _dbContext.Words.Where(w => w.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task UpdateAsync(Word entity)
        {
            _dbContext.Entry(entity).State = entity.Id == default ? EntityState.Added : EntityState.Modified;
            await _dbContext.SaveChangesAsync();
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