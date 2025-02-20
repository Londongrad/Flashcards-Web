using Flashcards.Domain.Entities;
using FlashcardsWEB.Domain.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsWEB.Domain.Repositories.EntityFramework
{
    public class SetRepository : IRepository<Set>
    {
        private readonly ApplicationDbContext _dbContext;

        public SetRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task DeleteAsync(int id)
        {
            await _dbContext.Sets.Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Set>> GetAllAsync()
        {
            return await _dbContext.Sets.Include(s => s.Words).ToListAsync();
        }

        public async Task<Set?> GetAsync(int id)
        {
            return await _dbContext.Sets.Include(s => s.Words).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Set entity)
        {
            _dbContext.Entry(entity).State = entity.Id == default ? EntityState.Added : EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
    }
}