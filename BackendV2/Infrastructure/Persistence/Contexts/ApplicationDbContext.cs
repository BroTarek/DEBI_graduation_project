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
        public DbSet<PlaylistVideoItem> PlaylistVideoItems { get; set; }
        public DbSet<Subscription> Subscriptions { get; set; }
        public DbSet<UserInteraction> UserInteractions { get; set; }
        public DbSet<WatchHistory> WatchHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(YDbContextDbContext).Assembly);
           
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        }
    }
}