using YouTubeClone.Domain.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Domain.Entities.Playlists;
using YouTubeClone.Domain.Entities.Subscriptions;
using YouTubeClone.Domain.Entities.WatchHistories;
using YouTubeClone.Domain.Aggregates.Videos;

namespace YouTubeClone.Persistance.Contexts
{
    public class YouTubeCloneDbContext(DbContextOptions<YouTubeCloneDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Video> Videos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(YouTubeCloneDbContext).Assembly);

            // Configure Channel entity
            builder.Entity<Channel>().HasKey(x => x.Id);
            builder.Entity<Channel>().OwnsOne(c => c.ChannelProfile);

            // Configure Video entity
            builder.Entity<Video>().HasKey(x => x.Id);
            builder.Entity<Video>().OwnsOne(v => v.video_Basics);
            builder.Entity<Video>().OwnsOne(v => v.video_Descriptive);
            builder.Entity<Video>().OwnsOne(v => v.video_Technical_details);
            builder.Entity<Video>().OwnsOne(v => v.Temporal_Metadata);
            builder.Entity<Video>().OwnsOne(v => v.VideoStats);

            // Configure Comment entity
            builder.Entity<Comment>().HasKey(x => x.Id);
            builder.Entity<Comment>()
                .HasOne(c => c.ParentComment)
                .WithMany(c => c.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Playlist entities (TPH)
            builder.Entity<Playlist>().HasKey(x => x.Id);
            builder.Entity<Playlist>()
                .HasMany(p => p.videos)
                .WithMany(v => v.Playlists)
                .UsingEntity<Dictionary<string, object>>(
                    "PlaylistVideo",
                    j => j.HasOne<Video>().WithMany().HasForeignKey("videosId").OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<Playlist>().WithMany().HasForeignKey("PlaylistsId").OnDelete(DeleteBehavior.Cascade)
                );

            // Configure Subscription entity
            builder.Entity<Subscription>().HasKey(x => x.Id);

            // Configure WatchHistory entity
            builder.Entity<WatchHistory>().HasKey(x => x.Id);
            builder.Entity<WatchHistory>()
                .HasMany(wh => wh.videos)
                .WithMany(v => v.WatchHistories)
                .UsingEntity<Dictionary<string, object>>(
                    "WatchHistoryVideo",
                    j => j.HasOne<Video>().WithMany().HasForeignKey("videosId").OnDelete(DeleteBehavior.Restrict),
                    j => j.HasOne<WatchHistory>().WithMany().HasForeignKey("WatchHistoriesId").OnDelete(DeleteBehavior.Cascade)
                );

            // Configure Post entity
            builder.Entity<Post>().HasKey(x => x.Id);

            // Configure 1-to-1 relationships with ApplicationUser
            builder.Entity<ApplicationUser>()
                .HasOne<Channel>()
                .WithOne(c => c.Owner)
                .HasForeignKey<Channel>(c => c.OwnerId);

            builder.Entity<ApplicationUser>()
                .HasOne<WatchHistory>()
                .WithOne(w => w.owner)
                .HasForeignKey<WatchHistory>(w => w.OwnerId);

            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        }
    }
}
