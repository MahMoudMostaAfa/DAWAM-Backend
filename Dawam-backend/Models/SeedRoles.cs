using Microsoft.AspNetCore.Identity;
namespace Dawam_backend.Models
{
    public class SeedRoles
    {
        public static void Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roleNames = new[] { "Admin", "JobPoster", "JobApplier" };

            foreach (var roleName in roleNames)
            {
                var roleExist = roleManager.RoleExistsAsync(roleName).Result;
                if (!roleExist)
                {
                    var role = new IdentityRole(roleName);
                    roleManager.CreateAsync(role).Wait();
                }
            }
        }
    }
}
