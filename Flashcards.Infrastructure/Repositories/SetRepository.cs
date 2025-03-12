using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Infrastructure.Repositories
{
    public class SetRepository(ApplicationDbContext dbContext) : ISetRepository
    {
        public async Task DeleteAsync(int id)
        {
            await dbContext.Sets.Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Set>> GetAllAsync(string userId)
        {
            return await dbContext.Sets.Where(s => s.UserId == userId).Include(s => s.Words).ToListAsync();
        }

        public async Task<Set?> GetAsync(int id)
        {
            return await dbContext.Sets.Include(s => s.Words).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Set set)
        {
            await dbContext.Sets.Where(s => s.Id == set.Id)
                .ExecuteUpdateAsync(sp => sp
                .SetProperty(s => s.Name, set.Name)
                );
        }

        public async Task AddAsync(Set set)
        {
            await dbContext.Sets.AddAsync(set);
            await dbContext.SaveChangesAsync();
        }
    }
}