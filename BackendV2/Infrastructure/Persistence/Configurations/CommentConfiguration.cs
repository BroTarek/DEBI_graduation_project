using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.ValueObjects;

namespace Makanak.Persistance.Configurations
{
    public class CommentConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder.ToTable("Comments");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, value => new CommentId(value))
                .IsRequired();

            builder.Property(c => c.AuthorId)
                .HasConversion(id => id.Value, value => new UserId(value))
                .IsRequired();

            builder.Property(c => c.Content)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(c => c.ParentCommentId)
                .HasConversion(id => id != null ? id.Value : (System.Guid?)null, value => value.HasValue ? new CommentId(value.Value) : null);

            // Self-referencing relationship for Replies
            builder.HasMany(c => c.Replies)
                   .WithOne()
                   .HasForeignKey(c => c.ParentCommentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
