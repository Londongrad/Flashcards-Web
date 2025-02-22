using FlashcardsWEB.Domain.Configurations;
using FlashcardsWEB.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FlashcardsWEB.Domain.Data
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