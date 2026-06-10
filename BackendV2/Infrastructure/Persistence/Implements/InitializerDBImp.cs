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
            
        }
    }
}