using Dawam_backend.Data;
using Dawam_backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Dawam_backend.Seeders
{
    public static class DataSeeder
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            // Seed Roles
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await SeedRoles.InitializeAsync(roleManager);

            // Seed Admin User
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            await SeedAdminUserAsync(userManager);
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@dawam.com";
            var adminPassword = "Admin@12345";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin User",
                    IsActive = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        public static void SeedInitialJobs(ApplicationDbContext context)
        {
            if (!context.Jobs.Any())
            {
                var categoryId = context.Categories.FirstOrDefault()?.Id;
                var userId = context.Users.FirstOrDefault()?.Id;

                var jobs = new List<Job>
                {
                    new Job { /* Job properties */ },
                    new Job { /* Job properties */ }
                };

                context.Jobs.AddRange(jobs);
                context.SaveChanges();
            }
        }
    }
}