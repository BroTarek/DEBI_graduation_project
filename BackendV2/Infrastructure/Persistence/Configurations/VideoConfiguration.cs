using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;
using System.Text.Json;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Persistance.Configurations
{
    public class VideoConfiguration : IEntityTypeConfiguration<Video>
    {
        public void Configure(EntityTypeBuilder<Video> builder)
        {
            builder.ToTable("Videos");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .HasConversion(id => id.Value, value => new VideoId(value))
                .IsRequired();

            builder.Property(v => v.ChannelId)
                .HasConversion(id => id.Value, value => new ChannelId(value))
                .IsRequired();

            builder.Property(v => v.Title)
                .HasConversion(t => t.Value, value => new Title(value))
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(v => v.Description)
                .HasConversion(d => d.Value, value => new Description(value));

            builder.Property(v => v.Duration)
                .HasConversion(d => d.Seconds, value => new Duration(value));

            builder.Property(v => v.ThumbnailUrl)
                .HasConversion(t => t.Value, value => new ThumbnailUrl(value));

            builder.Property(v => v.Category)
                .HasConversion(c => c.Value, value => new Category(value));

            builder.Property(v => v.PrivacyStatus)
                .HasConversion<string>(); // Store enum as string

            builder.Property(v => v.Tags)
                .HasConversion(
                    tags => JsonSerializer.Serialize(tags, (JsonSerializerOptions)null),
                    json => JsonSerializer.Deserialize<List<Tag>>(json, (JsonSerializerOptions)null)
                );

            // Relationships
            builder.HasMany(v => v.Comments)
                   .WithOne()
                   // In Domain, Comment doesn't have VideoId, so EF uses shadow property or we map it
                   // If Comment doesn't have VideoId property, EF will create 'VideoId' shadow FK
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);
                   
            builder.HasOne<YouTubeClone.Domain.Aggregates.Channels.Channel>()
                   .WithMany()
                   .HasForeignKey(v => v.ChannelId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
