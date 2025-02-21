using FlashcardsWEB.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FlashcardsWEB.Domain.Configurations
{
    public class WordConfiguration : IEntityTypeConfiguration<Word>
    {
        public void Configure(EntityTypeBuilder<Word> builder)
        {
            builder.HasKey(c => c.Id);

            builder.HasIndex(c => c.Name).IsUnique();

            builder.Property(c => c.Name)
                .IsRequired().HasMaxLength(100);

            builder.Property(c => c.Definition)
                .IsRequired().HasMaxLength(777);

            builder.Property(c => c.ImagePath)
                .HasMaxLength(777);

            builder.Property(c => c.IsFavorite)
                .HasDefaultValue(false);

            builder.Property(c => c.IsLastWord)
                .HasDefaultValue(false);
        }
    }
}