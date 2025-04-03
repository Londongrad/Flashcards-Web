using Flashcards.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Flashcards.Infrastructure.Configurations
{
    internal class SetConfiguration : IEntityTypeConfiguration<Set>
    {
        public void Configure(EntityTypeBuilder<Set> builder)
        {
            builder.HasKey(c => c.Id);

            builder
                .HasMany(s => s.Words)
                .WithOne(w => w.Set)
                .HasForeignKey(w => w.SetId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(c => c.Name)
                .IsRequired().HasMaxLength(20);

            builder.HasIndex(s => s.Name).IsUnique();
        }
    }
}