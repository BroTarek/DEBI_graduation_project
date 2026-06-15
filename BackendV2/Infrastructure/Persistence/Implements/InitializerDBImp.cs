using YouTubeClone.Domain.Contracts.InitializerDB;
using YouTubeClone.Domain.Models.Identity;
using YouTubeClone.Persistance.Contexts;
using YouTubeClone.Persistance.Seeds;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace YouTubeClone.Persistance.Implements.InitializerImplement
{
    public class DbInitialized(YouTubeCloneDbContext YouTubeCloneDbContext , RoleManager<IdentityRole> roleManager , UserManager<ApplicationUser> userManager) : IDbInitializer
    {
        public async Task DataSeedAsync()
        {
            try
            {
                var pendingMigrations = await YouTubeCloneDbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations != null && pendingMigrations.Any())
                    await YouTubeCloneDbContext.Database.MigrateAsync();
            }
            catch (Exception)
            {
                // Log the exception or handle it as needed
                throw;
            }
            await SeederAsync.SeedRolesAsync(roleManager);

            await SeederAsync.SeedAdminsAsync(userManager);

            await SeedChannelsAndContentAsync();
        }

        private async Task SeedChannelsAndContentAsync()
        {
            if (!await YouTubeCloneDbContext.Channels.AnyAsync())
            {
                var userIdGuid = new Guid("22222222-2222-2222-2222-222222222222");
                var userId = new YouTubeClone.Domain.ValueObjects.UserId(userIdGuid);

                // Ensure domain user exists
                var domainUser = await YouTubeCloneDbContext.DomainUsers.FindAsync(userId);
                if (domainUser == null)
                {
                    var credentials = new YouTubeClone.Domain.Aggregates.Users.UserCredentials("USER@YouTubeClone.SITE", "hashed", "salt", "mfa");
                    var profileInfo = new YouTubeClone.Domain.Aggregates.Users.UserProfileInfo("Test User", "Bio of test user", "https://ui-avatars.com/api/?name=Test+User&background=random", "dark", "tech");

                    var watchHistoryId = new YouTubeClone.Domain.ValueObjects.WatchHistoryId(Guid.NewGuid());
                    var watchHistory = new YouTubeClone.Domain.Aggregates.WatchHistories.WatchHistory(watchHistoryId, null!);

                    var likedVideosPlaylistId = new YouTubeClone.Domain.ValueObjects.PlaylistId(Guid.NewGuid());
                    var likedVideosPlaylist = new YouTubeClone.Domain.Aggregates.Playlists.LikedVideosPlaylist(likedVideosPlaylistId, YouTubeClone.Domain.Aggregates.Accessibility.PUBLIC, userIdGuid.ToString());

                    var subscriptionsId = new YouTubeClone.Domain.ValueObjects.SubscriptionId(Guid.NewGuid());
                    var subscriptions = new YouTubeClone.Domain.Aggregates.Subscriptions.Subscriptions(subscriptionsId, userIdGuid.ToString());

                    domainUser = new YouTubeClone.Domain.Aggregates.Users.User(
                        userId,
                        credentials,
                        profileInfo,
                        watchHistory,
                        likedVideosPlaylist,
                        subscriptions
                    );

                    // Fix circular reference using reflection
                    var ownerProp = typeof(YouTubeClone.Domain.Aggregates.WatchHistories.WatchHistory)
                        .GetProperty("Owner", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    ownerProp?.SetValue(watchHistory, domainUser);

                    await YouTubeCloneDbContext.DomainUsers.AddAsync(domainUser);
                }

                // Create Channel
                var channelId = new YouTubeClone.Domain.ValueObjects.ChannelId(new Guid("22222222-2222-2222-2222-222222222223"));
                var channelProfile = new YouTubeClone.Domain.Aggregates.Channels.ChannelProfile(
                    "This is a seeded test channel with all the standard entities.",
                    "https://youtube.com?channel=test",
                    "Test Channel",
                    "https://ui-avatars.com/api/?name=Test+Channel&background=random",
                    "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe?w=1500&auto=format&fit=crop", // Banner
                    1000
                );
                
                var channel = new YouTubeClone.Domain.Aggregates.Channels.Channel(channelId, domainUser, channelProfile);
                domainUser.AssignChannel(channel);

                await YouTubeCloneDbContext.Channels.AddAsync(channel);

                // Seed some videos
                var videoIdGuid = Guid.NewGuid();
                var videoId = new YouTubeClone.Domain.ValueObjects.VideoId(videoIdGuid);
                var basics = new YouTubeClone.Domain.Aggregates.Videos.video_Basics(
                    videoIdGuid.ToString(), 
                    "https://images.unsplash.com/photo-1611162617213-7d7a39e9b1d7?w=640", 
                    "https://www.w3schools.com/html/mov_bbb.mp4", 
                    YouTubeClone.Domain.Aggregates.Accessibility.PUBLIC
                );
                var descriptive = new YouTubeClone.Domain.Aggregates.Videos.video_Descriptive(
                    "Introduction to Backend Development", 
                    "Learn the basics of clean architecture and entity framework core.", 
                    "Education", 
                    new[] { "csharp", "dotnet", "cleanarchitecture" }
                );
                var technical = new YouTubeClone.Domain.Aggregates.Videos.video_Technical_details(180, "1080p", 4500000, "mp4", "h264", "aac", 30f, 5000);
                var temporal = new YouTubeClone.Domain.Aggregates.Videos.Temporal_Metadata(DateTime.UtcNow.AddDays(-5), DateTime.UtcNow.AddDays(-5), "Uploaded");
                var stats = new YouTubeClone.Domain.Aggregates.Videos.VideoStats(3452, 120, 15);
                var video = new YouTubeClone.Domain.Aggregates.Videos.Video(videoId, "22222222-2222-2222-2222-222222222223", basics, descriptive, technical, temporal, stats);

                var videoIdGuid2 = Guid.NewGuid();
                var videoId2 = new YouTubeClone.Domain.ValueObjects.VideoId(videoIdGuid2);
                var basics2 = new YouTubeClone.Domain.Aggregates.Videos.video_Basics(
                    videoIdGuid2.ToString(), 
                    "https://images.unsplash.com/photo-1611162616305-c69b3fa7fbe0?w=640", 
                    "https://www.w3schools.com/html/movie.mp4", 
                    YouTubeClone.Domain.Aggregates.Accessibility.PUBLIC
                );
                var descriptive2 = new YouTubeClone.Domain.Aggregates.Videos.video_Descriptive(
                    "Building Responsive Web Apps with Glassmorphism", 
                    "Create stunning and premium UI/UX aesthetics using CSS and vanilla JS.", 
                    "Design", 
                    new[] { "css", "webdesign", "ux" }
                );
                var technical2 = new YouTubeClone.Domain.Aggregates.Videos.video_Technical_details(240, "1080p", 6200000, "mp4", "h264", "aac", 30f, 5000);
                var temporal2 = new YouTubeClone.Domain.Aggregates.Videos.Temporal_Metadata(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(-2), "Uploaded");
                var stats2 = new YouTubeClone.Domain.Aggregates.Videos.VideoStats(1820, 95, 8);
                var video2 = new YouTubeClone.Domain.Aggregates.Videos.Video(videoId2, "22222222-2222-2222-2222-222222222223", basics2, descriptive2, technical2, temporal2, stats2);

                await YouTubeCloneDbContext.Videos.AddAsync(video);
                await YouTubeCloneDbContext.Videos.AddAsync(video2);

                // Seed posts
                var post1 = new YouTubeClone.Domain.Aggregates.Channels.Post(
                    new YouTubeClone.Domain.Aggregates.Channels.PostId(Guid.NewGuid()),
                    "22222222-2222-2222-2222-222222222223",
                    "Hey everyone! Welcome to my official YouTube Clone channel. We just uploaded two new videos on Backend Development and CSS Glassmorphism. Check them out under the Videos tab!",
                    YouTubeClone.Domain.Aggregates.Accessibility.PUBLIC
                );

                var post2 = new YouTubeClone.Domain.Aggregates.Channels.Post(
                    new YouTubeClone.Domain.Aggregates.Channels.PostId(Guid.NewGuid()),
                    "22222222-2222-2222-2222-222222222223",
                    "We are planning an upcoming live stream to walk through the system architecture. Let us know in the comments what topics you'd like us to cover!",
                    YouTubeClone.Domain.Aggregates.Accessibility.PUBLIC
                );

                await YouTubeCloneDbContext.Set<YouTubeClone.Domain.Aggregates.Channels.Post>().AddAsync(post1);
                await YouTubeCloneDbContext.Set<YouTubeClone.Domain.Aggregates.Channels.Post>().AddAsync(post2);

                await YouTubeCloneDbContext.SaveChangesAsync();
            }
        }
    }
}