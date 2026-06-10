using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Aggregates.Users;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Persistance.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("DomainUsers");

            builder.HasKey(u => u.Id);
            
            builder.Property(u => u.Id)
                .HasConversion(id => id.Value, value => new UserId(value))
                .IsRequired();

            builder.Property(u => u.Username)
                .HasConversion(u => u.Value, value => new Username(value))
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(u => u.Email)
                .HasConversion(e => e.Value, value => new Email(value))
                .IsRequired();

            builder.Property(u => u.PasswordHash)
                .HasConversion(p => p.Value, value => new PasswordHash(value));

            builder.Property(u => u.AvatarUrl)
                .HasConversion(a => a.Value, value => new AvatarUrl(value));

            // Relationships
            builder.HasMany(u => u.Channels)
                   .WithOne()
                   .HasForeignKey(c => c.OwnerId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
