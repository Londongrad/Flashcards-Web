using Flashcards.Domain.Entities;
using Flashcards.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Flashcards.Infrastructure.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<IdentityUser>(options)
    {
        public DbSet<Set> Sets { get; set; } = null!;
        public DbSet<Word> Words { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new SetConfiguration());
            builder.ApplyConfiguration(new WordConfiguration());
            base.OnModelCreating(builder);
        }
    }
}