//using Microsoft.AspNetCore.Identity;
//namespace Dawam_backend.Models
//{
//    public class SeedRoles
//    {
//        public static void Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
//        {
//            var roleNames = new[] { "Admin", "JobPoster", "JobApplier" };

//            foreach (var roleName in roleNames)
//            {
//                var roleExist = roleManager.RoleExistsAsync(roleName).Result;
//                if (!roleExist)
//                {
//                    var role = new IdentityRole(roleName);
//                    roleManager.CreateAsync(role).Wait();
//                }
//            }
//        }
//    }
//}
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Dawam_backend.Models
{
    public class SeedRoles
    {
        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {
            var roleNames = new[] { "Admin", "JobPoster", "JobApplier" };
            var loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
            var logger = loggerFactory.CreateLogger<SeedRoles>();

            foreach (var roleName in roleNames)
            {
                await CreateRoleIfNotExists(roleManager, roleName, logger);
            }
        }

        private static async Task CreateRoleIfNotExists(
            RoleManager<IdentityRole> roleManager,
            string roleName,
            ILogger logger)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                logger.LogInformation("Creating {role} role", roleName);
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    logger.LogError("Failed to create {role} role. Errors: {errors}",
                        roleName,
                        string.Join(", ", result.Errors));
                }
            }
            else
            {
                logger.LogInformation("{role} role already exists", roleName);
            }
        }
    }
}