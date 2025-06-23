using Flashcards.Application.DTOs;
using Flashcards.Application.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Infrastructure.Repositories
{
    public class SetRepository(ApplicationDbContext dbContext) : ISetRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        private IQueryable<Set> GetUserSets(string userId)
        {
            return _dbContext.Sets.Where(s => s.UserId == userId);
        }

        public async Task DeleteAsync(int id, string userId)
        {
            await GetUserSets(userId)
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<SetDTO>> GetAllSummariesAsync(string userId)
        {
            return await GetUserSets(userId)
                .Select(s => new SetDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    HasImages = s.Words.Any(w => w.ImagePath != null),
                    WordCount = s.Words.Count
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Set?> GetAsync(int id, string userId)
        {
            return await GetUserSets(userId)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Set?> GetWithWordsAsync(int id, string userId)
        {
            return await GetUserSets(userId)
                .Include(s => s.Words)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Set set)
        {
            await GetUserSets(set.UserId)
                .Where(s => s.Id == set.Id)
                .ExecuteUpdateAsync(sp => sp
                    .SetProperty(s => s.Name, set.Name)
                );
        }

        public async Task AddAsync(Set set)
        {
            await _dbContext.Sets.AddAsync(set);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> IsNotUnique(string name, int id, string userId)
        {
            var query = GetUserSets(userId);

            if (id == 0)
                return await query.AnyAsync(s => s.Name == name);

            return await query.AnyAsync(s => s.Name == name && s.Id != id);
        }

        public async Task DeleteAllAsync(string userId)
        {
            await GetUserSets(userId).ExecuteDeleteAsync();
        }

        public async Task<Set?> GetFavoriteAsync(int id, string userId)
        {
            return await GetUserSets(userId)
                .Where(s => s.Id == id)
                .Select(s => new Set(s.Id, s.Name, s.UserId)
                    {
                        Words = s.Words
                            .Where(w => w.IsFavorite)
                            .ToList()
                    })
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }
    }
}