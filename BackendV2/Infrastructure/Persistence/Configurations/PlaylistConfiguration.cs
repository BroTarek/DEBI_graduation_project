using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Aggregates.Playlists;
using YouTubeClone.Domain.ValueObjects;

namespace Makanak.Persistance.Configurations
{
    public class PlaylistConfiguration : IEntityTypeConfiguration<Playlist>
    {
        public void Configure(EntityTypeBuilder<Playlist> builder)
        {
            builder.ToTable("Playlists");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .HasConversion(id => id.Value, value => new PlaylistId(value))
                .IsRequired();

            builder.Property(p => p.ChannelId)
                .HasConversion(id => id.Value, value => new ChannelId(value))
                .IsRequired();

            builder.Property(p => p.Name)
                .HasConversion(n => n.Value, value => new ChannelName(value))
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasConversion(d => d.Value, value => new Description(value));

            builder.Property(p => p.ThumbnailUrl)
                .HasConversion(t => t.Value, value => new ThumbnailUrl(value));

            // Configure PlaylistVideoItem collection
            builder.OwnsMany(p => p.VideoItems, itemBuilder =>
            {
                itemBuilder.ToTable("PlaylistVideoItems");
                
                itemBuilder.HasKey(vi => new { vi.VideoId, PlaylistId = EF.Property<System.Guid>(vi, "PlaylistId") }); // Composite Key if needed, but VideoId is PK for entity. Wait, PlaylistVideoItem inherits from Entity<VideoId>.

                itemBuilder.Property(vi => vi.Id)
                    .HasConversion(id => id.Value, value => new VideoId(value));
                    
                itemBuilder.WithOwner().HasForeignKey("PlaylistId"); // Shadow property for FK
                
                itemBuilder.Property(vi => vi.Position).IsRequired();
            });
            
            // Add metadata for navigation to work properly without setter
            builder.Metadata.FindNavigation(nameof(Playlist.VideoItems))
                   ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
