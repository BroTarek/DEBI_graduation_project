using YouTubeClone.Domain.EnumsHelper.User; 
using YouTubeClone.Domain.Models.Identity;
using YouTubeClone.Domain.Entities.Channels;
using YouTubeClone.Domain.Entities.Playlists;
using YouTubeClone.Domain.Entities.Videos;
using YouTubeClone.Persistance.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace YouTubeClone.Persistance.Seeds
{
    public static class SeederAsync
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetNames(typeof(UserTypes)))
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdminsAsync(UserManager<ApplicationUser> userManager)
        {
            if (!userManager.Users.Any())
            {
                var seedUsers = new List<ApplicationUser>
                {
                    new ApplicationUser
                    {
                        Id = "11111111-1111-1111-1111-111111111111", 
                        UserStatus = UserStatus.Active,
                        DateOfBirth = new DateTime(2000, 1, 1),
                        UserType = UserTypes.Admin,
                        Name = "System Admin",
                        PhoneNumber = "01225869788",
                        UserName = "admin@youtubeclone.site",
                        Email = "admin@youtubeclone.site",
                        EmailConfirmed = true,
                    },
                    new ApplicationUser
                    {
                        Id = "22222222-2222-2222-2222-222222222222", 
                        UserStatus = UserStatus.Active,
                        DateOfBirth = new DateTime(2000, 1, 1),
                        UserType = UserTypes.Standard,
                        Name = "Test User",
                        PhoneNumber = "01225869789",
                        UserName = "user@youtubeclone.site",
                        Email = "user@youtubeclone.site",
                        EmailConfirmed = true,
                    }
                };

                string defaultPassword = "AdminPassword@123";

                foreach (var user in seedUsers)
                {
                    var result = await userManager.CreateAsync(user, defaultPassword);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, user.UserType.ToString());
                    }
                }
            }
        }

        public static async Task SeedChannelsAsync(YouTubeCloneDbContext context)
        {
            if (!await context.Channels.AnyAsync())
            {
                var channelsToSeed = new List<Channel>();

                var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Id == "11111111-1111-1111-1111-111111111111");
                var standardUser = await context.Users.FirstOrDefaultAsync(u => u.Id == "22222222-2222-2222-2222-222222222222");

                if (adminUser != null)
                {
                    channelsToSeed.Add(new Channel
                    {
                        Id = Guid.Parse("11111111-1111-1111-1111-111111111112"),
                        OwnerId = adminUser.Id,
                        Owner = adminUser, 
                        ChannelProfile = new ChannelProfile
                        {
                            name = "Admin Dev Central",
                            channelsDescription = "Official system administration and announcements channel.",
                            subscribersCount = 1337,
                            avatar = "https://api.dicebear.com/7.x/bottts/svg?seed=admin",
                            greaterImg = "https://images.unsplash.com/photo-1618005182384-a83a8bd57fbe",
                            links = "https://youtubeclone.site"
                        },
                        videos = new List<Video>(),
                        posts = new List<Post>(),
                        channelPlaylists = new List<ChannelPlaylist>()
                    });
                }

                if (standardUser != null)
                {
                    channelsToSeed.Add(new Channel
                    {
                        Id = Guid.Parse("22222222-2222-2222-2222-222222222223"),
                        OwnerId = standardUser.Id,
                        Owner = standardUser,
                        ChannelProfile = new ChannelProfile
                        {
                            name = "Test Creator Space",
                            channelsDescription = "Welcome to my test channel where I publish staging content!",
                            subscribersCount = 12,
                            avatar = "https://api.dicebear.com/7.x/avataaars/svg?seed=testuser",
                            greaterImg = "https://images.unsplash.com/photo-1707343843437-caacff5cfa74",
                            links = "https://github.com/testuser"
                        },
                        videos = new List<Video>(),
                        posts = new List<Post>(),
                        channelPlaylists = new List<ChannelPlaylist>()
                    });
                }

                if (channelsToSeed.Any())
                {
                    await context.Channels.AddRangeAsync(channelsToSeed);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}