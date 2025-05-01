using Dawam_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dawam_backend.Controllers
{
    
        [Authorize(Roles = "Admin")]
        [Route("api/[controller]")]
        [ApiController]
        public class UsersController : ControllerBase
        {
            private readonly UserManager<ApplicationUser> _userManager;

            public UsersController(UserManager<ApplicationUser> userManager)
            {
                _userManager = userManager;
            }

            [HttpGet]
            public async Task<IActionResult> GetAllUsers()
            {
            var users = _userManager.Users.ToList();

            var nonAdminUsers = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin"))
                {
                    nonAdminUsers.Add(new
                    {
                        user.Id,
                        user.FullName,
                        user.Email,
                        user.UserName,
                        user.IsActive,
                        user.CreatedAt,
                        role=roles
                    });
                }
            }

            return Ok(nonAdminUsers);
            }


             [HttpDelete("{UserId}")]
             public async Task<IActionResult> DeleteUser(string UserId)
        {
           

            // Find user in the database
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            if(!user.IsActive)
            {
                return NotFound(new { message = "User already deactivated." });
            }
            // Set IsActive to false
            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Your account has been deactivated." });
        }

    }

}
