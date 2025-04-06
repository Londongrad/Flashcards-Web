using Flashcards.Application.Common.Interfaces;
using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Flashcards.Infrastructure.Repositories
{
    public class SetRepository : ISetRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly string _userId;

        public SetRepository(ApplicationDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        }

        public async Task DeleteAsync(int id)
        {
            await _dbContext.Sets.Where(s => s.Id == id)
                .ExecuteDeleteAsync();
        }

        public async Task<IEnumerable<Set>> GetAllAsync()
        {
            return await _dbContext.Sets.Where(s => s.UserId == _userId).Include(s => s.Words).AsNoTracking().ToListAsync();
        }

        public async Task<Set?> GetAsync(int id)
        {
            return await _dbContext.Sets.Where(s => s.UserId == _userId).Include(s => s.Words).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Set set)
        {
            await _dbContext.Sets.Where(s => s.Id == set.Id)
                .ExecuteUpdateAsync(sp => sp
                .SetProperty(s => s.Name, set.Name)
                );
        }

        public async Task AddAsync(Set set)
        {
            await _dbContext.Sets.AddAsync(set);
            await _dbContext.SaveChangesAsync();
        }
    }
}