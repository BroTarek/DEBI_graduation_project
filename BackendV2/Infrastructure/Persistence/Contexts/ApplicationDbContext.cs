using YouTubeClone.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YouTubeClone.Domain.Aggregates.Users;
using YouTubeClone.Domain.Aggregates.Channels;
using YouTubeClone.Domain.Aggregates.Videos;
using YouTubeClone.Domain.Aggregates.Playlists;
using YouTubeClone.Domain.Aggregates.Subscriptions;
using YouTubeClone.Domain.Aggregates.Interactions;
using YouTubeClone.Domain.Aggregates.WatchHistories;

namespace YouTubeClone.Persistance.Contexts
{
    public class YouTubeCloneDbContext(DbContextOptions<YouTubeCloneDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<User> DomainUsers { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Subscriptions> Subscriptions { get; set; }
        public DbSet<UserInteraction> UserInteractions { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(YouTubeCloneDbContext).Assembly);

            // Configure primary keys and strong-typed IDs conversions
            builder.Entity<User>().HasKey(x => x.Id);
            builder.Entity<User>().Property(x => x.Id).HasConversion(id => id.Value, val => new YouTubeClone.Domain.ValueObjects.UserId(val));
            builder.Entity<User>().OwnsOne(u => u.Credentials);
            builder.Entity<User>().OwnsOne(u => u.ProfileInfo);

            builder.Entity<Channel>().HasKey(x => x.Id);
            builder.Entity<Channel>().Property(x => x.Id).HasConversion(id => id.Value, val => new YouTubeClone.Domain.ValueObjects.ChannelId(val));
            builder.Entity<Channel>().OwnsOne(c => c.Profile);

            builder.Entity<Video>().HasKey(x => x.Id);
            builder.Entity<Video>().Property(x => x.Id).HasConversion(id => id.Value, val => new YouTubeClone.Domain.ValueObjects.VideoId(val));
            builder.Entity<Video>().OwnsOne(v => v.Basics);
            builder.Entity<Video>().OwnsOne(v => v.Descriptive);
            builder.Entity<Video>().OwnsOne(v => v.TechnicalDetails);
            builder.Entity<Video>().OwnsOne(v => v.TemporalMetadata);
            builder.Entity<Video>().OwnsOne(v => v.Stats);

            builder.Entity<Comment>().HasKey(x => x.Id);
            builder.Entity<Comment>().Property(x => x.Id).HasConversion(id => id.Value, val => new YouTubeClone.Domain.ValueObjects.CommentId(val));
            builder.Entity<Comment>().Property(x => x.ParentCommentId)
                .HasConversion(
                    id => id == null ? (Guid?)null : id.Value,
                    val => val == null ? null : new YouTubeClone.Domain.ValueObjects.CommentId(val.Value)
                );
            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany()
                .HasForeignKey(c => c.ParentCommentId);

            builder.Entity<Playlist>().HasKey(x => x.Id);
            builder.Entity<Playlist>().Property(x => x.Id).HasConversion(id => id.Value, val => new YouTubeClone.Domain.ValueObjects.PlaylistId(val));

            builder.Entity<Subscriptions>().HasKey(x => x.Id);
            builder.Entity<Subscriptions>().Property(x => x.Id).HasConversion(id => id.Value, val => new YouTubeClone.Domain.ValueObjects.SubscriptionId(val));

            builder.Entity<UserInteraction>().HasKey(x => x.Id);
            builder.Entity<UserInteraction>().Property(x => x.Id).HasConversion(id => id.Value, val => new UserInteractionId(val));

            builder.Entity<WatchHistory>().HasKey(x => x.Id);
            builder.Entity<WatchHistory>().Property(x => x.Id).HasConversion(id => id.Value, val => new YouTubeClone.Domain.ValueObjects.WatchHistoryId(val));

            builder.Entity<Post>().HasKey(x => x.Id);
            builder.Entity<Post>().Property(x => x.Id).HasConversion(id => id.Value, val => new PostId(val));

            // Configure 1-to-1 relationships to resolve EF Core conventions
            builder.Entity<User>()
                .HasOne(u => u.Channel)
                .WithOne(c => c.Owner)
                .HasForeignKey<Channel>("OwnerId"); // Shadow property

            builder.Entity<User>()
                .HasOne(u => u.WatchHistory)
                .WithOne(w => w.Owner)
                .HasForeignKey<WatchHistory>("OwnerId"); // Shadow property

            builder.Entity<User>()
                .HasOne(u => u.LikedVideosPlaylist)
                .WithOne()
                .HasForeignKey<LikedVideosPlaylist>("DomainUserId"); // Shadow property to avoid clashing with string OwnerId

            builder.Entity<User>()
                .HasOne(u => u.Subscriptions)
                .WithOne()
                .HasForeignKey<Subscriptions>("DomainUserId"); // Shadow property to avoid clashing with string OwnerId
           
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        }
    }
}