using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.ValueObjects;

namespace YouTubeClone.Persistance.Configurations
{
    public class ChannelConfiguration : IEntityTypeConfiguration<Channel>
    {
        public void Configure(EntityTypeBuilder<Channel> builder)
        {
            builder.ToTable("Channels");

            builder.HasKey(c => c.Id);
            
            builder.Property(c => c.Id)
                .HasConversion(id => id.Value, value => new ChannelId(value))
                .IsRequired();

            builder.Property(c => c.OwnerId)
                .HasConversion(id => id.Value, value => new UserId(value))
                .IsRequired();

            builder.Property(c => c.Name)
                .HasConversion(n => n.Value, value => new ChannelName(value))
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasConversion(d => d.Value, value => new ChannelDescription(value));

            // Relationships can be configured from User side as well, but no complex navigations required here 
            // since Channel doesn't contain a direct reference to List<Video> in its aggregate (Video has ChannelId).
        }
    }
}
