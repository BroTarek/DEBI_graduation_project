using YouTubeClone.Domain.EnumsHelper.User; 
using YouTubeClone.Domain.Models.Identity;
using YouTubeClone.Persistance.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

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
            string adminRole = UserTypes.Admin.ToString();

            var admins = await userManager.GetUsersInRoleAsync(adminRole);

            if (!admins.Any())
            {
                var adminUsers = new List<ApplicationUser>
        {
            new ApplicationUser
            {
                UserStatus = UserStatus.Active,
                DateOfBirth = new DateTime(2000, 1, 1),
                UserType = UserTypes.Admin,
                Name = "System Admin",
                PhoneNumber = "01225869788",
                UserName = "ADMIN@YouTubeClone.SITE",
                Email = "admin@YouTubeClone.site",
                EmailConfirmed = true,
            }
        };

                string defaultPassword = "AdminPassword@123";

                foreach (var admin in adminUsers)
                {
                    var result = await userManager.CreateAsync(admin, defaultPassword);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, adminRole);
                    }
                }
            }
        }
       
       
    }
}