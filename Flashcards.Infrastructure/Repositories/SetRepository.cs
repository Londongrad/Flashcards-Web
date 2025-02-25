using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Infrastructure.Repositories
{
    public class SetRepository(ApplicationDbContext dbContext) : IRepository<Set>
    {
        public async Task DeleteAsync(int id)
        {
            await dbContext.Sets.Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Set>> GetAllAsync()
        {
            return await dbContext.Sets.Include(s => s.Words).ToListAsync();
        }

        public async Task<Set?> GetAsync(int id)
        {
            return await dbContext.Sets.Include(s => s.Words).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Set entity)
        {
            dbContext.Entry(entity).State = entity.Id == default ? EntityState.Added : EntityState.Modified;
            await dbContext.SaveChangesAsync();
        }
    }
}