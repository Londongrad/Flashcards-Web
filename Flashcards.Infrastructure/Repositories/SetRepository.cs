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

        /// <summary>Returns a base query filtered by current user.</summary>
        public IQueryable<Set> GetUserSets(string userId)
        {
            return _dbContext.Sets.Where(s => s.UserId == userId);
        }

        /// <summary>Deletes a set with the specified ID if it belongs to the current user.</summary>
        public async Task DeleteAsync(int id, string userId)
        {
            await GetUserSets(userId)
                .Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }

        /// <summary>Retrieves all sets that belong to the current user without their associated words.</summary>
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

        /// <summary>Retrieves a specific set by ID if it belongs to the current user.</summary>
        public async Task<Set?> GetAsync(int id, string userId)
        {
            return await GetUserSets(userId)
                .Include(s => s.Words)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>Updates the name of a set if it belongs to the current user.</summary>
        public async Task UpdateAsync(Set set)
        {
            await GetUserSets(set.UserId)
                .Where(s => s.Id == set.Id)
                .ExecuteUpdateAsync(sp => sp
                    .SetProperty(s => s.Name, set.Name)
                );
        }

        /// <summary>Adds a new set for the current user.</summary>
        public async Task AddAsync(Set set)
        {
            await _dbContext.Sets.AddAsync(set);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>Checks whether a set name is not unique. <br/>
        /// If id == 0: check if any set with the same name exists. <br/>
        /// If id != 0: check if another set with the same name exists (used during update).</summary>
        public async Task<bool> IsNotUnique(string name, int id, string userId)
        {
            var query = GetUserSets(userId);

            if (id == 0)
                return await query.AnyAsync(s => s.Name == name);

            return await query.AnyAsync(s => s.Name == name && s.Id != id);
        }

        /// <summary>Deletes all sets belonging to the current user.</summary>
        public async Task DeleteAllAsync(string userId)
        {
            await GetUserSets(userId).ExecuteDeleteAsync();
        }
    }
}