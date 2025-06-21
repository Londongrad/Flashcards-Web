using Flashcards.Application.DTOs;
using Flashcards.Application.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Flashcards.Infrastructure.Repositories
{
    public class SetRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor) : ISetRepository
    {
        private readonly ApplicationDbContext _dbContext = dbContext;

        // Get the current logged-in user's ID from the HTTP context
        private readonly string _userId = httpContextAccessor.HttpContext.User
            .FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;

        /// <summary>Returns a base query filtered by current user.</summary>
        private IQueryable<Set> UserSets => _dbContext.Sets.Where(s => s.UserId == _userId);

        /// <summary>Deletes a set with the specified ID if it belongs to the current user.</summary>
        public async Task DeleteAsync(int id)
        {
            await UserSets.Where(s => s.Id == id).ExecuteDeleteAsync();
        }

        /// <summary>Retrieves all sets that belong to the current user without their associated words.</summary>
        public async Task<IEnumerable<SetDTO>> GetAllSummariesAsync()
        {
            return await UserSets
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
        public async Task<Set?> GetAsync(int id)
        {
            return await UserSets
                .Include(s => s.Words)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        /// <summary>Updates the name of a set if it belongs to the current user.</summary>
        public async Task UpdateAsync(Set set)
        {
            await UserSets
                .Where(s => s.Id == set.Id)
                .ExecuteUpdateAsync(sp => sp
                    .SetProperty(s => s.Name, set.Name)
                );
        }

        /// <summary>Adds a new set for the current user.</summary>
        public async Task AddAsync(Set set)
        {
            // Ensure the set is linked to the current user
            set.UserId = _userId;

            await _dbContext.Sets.AddAsync(set);
            await _dbContext.SaveChangesAsync();
        }

        /// <summary>Checks whether a set name is not unique. <br/>
        /// If id == 0: check if any set with the same name exists. <br/>
        /// If id != 0: check if another set with the same name exists (used during update).</summary>
        public async Task<bool> IsNotUnique(string name, int id)
        {
            var query = _dbContext.Sets.AsQueryable();

            if (id == 0)
                return await query.AnyAsync(s => s.Name == name && s.UserId == _userId);

            return await query.AnyAsync(s => s.Name == name && s.Id != id && s.UserId == _userId);
        }

        /// <summary>Deletes all sets belonging to the current user.</summary>
        public async Task DeleteAllAsync()
        {
            await UserSets.ExecuteDeleteAsync();
        }
    }
}