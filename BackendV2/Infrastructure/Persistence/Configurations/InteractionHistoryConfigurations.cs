using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using YouTubeClone.Domain.Aggregates.Interactions;
using YouTubeClone.Domain.Aggregates.Subscriptions;
using YouTubeClone.Domain.Aggregates.WatchHistories;
using YouTubeClone.Domain.ValueObjects;

namespace Makanak.Persistance.Configurations
{
    public class UserInteractionConfiguration : IEntityTypeConfiguration<UserInteraction>
    {
        public void Configure(EntityTypeBuilder<UserInteraction> builder)
        {
            builder.ToTable("UserInteractions");

            builder.HasKey(ui => ui.Id);
            
            builder.Property(ui => ui.Id)
                .HasConversion(id => id.Value, value => new UserInteractionId(value))
                .IsRequired();

            builder.Property(ui => ui.UserId)
                .HasConversion(id => id.Value, value => new UserId(value))
                .IsRequired();

            builder.Property(ui => ui.Type)
                .HasConversion<string>(); // Store enum as string

            // Configure InteractionTarget (Complex Property / Owned Type)
            builder.OwnsOne(ui => ui.Target, targetBuilder =>
            {
                targetBuilder.Property(t => t.Type).HasConversion<string>().HasColumnName("TargetType");
                targetBuilder.Property(t => t.Id).HasColumnName("TargetId");
            });
        }
    }

    public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
    {
        public void Configure(EntityTypeBuilder<Subscription> builder)
        {
            builder.ToTable("Subscriptions");

            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.Id)
                .HasConversion(id => id.Value, value => new SubscriptionId(value))
                .IsRequired();

            builder.Property(s => s.SubscriberId)
                .HasConversion(id => id.Value, value => new UserId(value))
                .IsRequired();

            builder.Property(s => s.ChannelId)
                .HasConversion(id => id.Value, value => new ChannelId(value))
                .IsRequired();
        }
    }

    public class WatchHistoryConfiguration : IEntityTypeConfiguration<WatchHistory>
    {
        public void Configure(EntityTypeBuilder<WatchHistory> builder)
        {
            builder.ToTable("WatchHistories");

            builder.HasKey(wh => wh.Id);
            
            builder.Property(wh => wh.Id)
                .HasConversion(id => id.Value, value => new WatchHistoryId(value))
                .IsRequired();

            builder.Property(wh => wh.UserId)
                .HasConversion(id => id.Value, value => new UserId(value))
                .IsRequired();

            builder.Property(wh => wh.VideoId)
                .HasConversion(id => id.Value, value => new VideoId(value))
                .IsRequired();
        }
    }
}
