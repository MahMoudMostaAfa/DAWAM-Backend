using Dawam_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                    {   user.Slug,
                        user.Id,
                        user.FullName,
                        user.Email,
                        user.UserName,
                        user.IsActive,
                        user.CreatedAt,
                        phone=user.PhoneNumber,
                        role=roles
                    });
                }
            }

            return Ok(nonAdminUsers);
            }


             [HttpDelete("{UserId}")]
             public async Task<IActionResult> DeActivateUser(string UserId)
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
           
            [HttpPost("{UserId}")]
             public async Task<IActionResult> ActivateUser(string UserId)
        {


            // Find user in the database
            var user = await _userManager.FindByIdAsync(UserId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }
            if (user.IsActive)
            {
                return NotFound(new { message = "User already activated." });
            }
            // Set IsActive to true
            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Your account has been activated." });
        }
        [AllowAnonymous]
        [HttpGet("{slug}")]
        public async Task<IActionResult> GetUserBySlug(string slug)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Slug == slug && u.IsActive);

            if (user == null)
                return NotFound(new {message="no found user"});

            return Ok(new
            {
                
                user.FullName,
                user.Email,
                user.Title,
                user.Bio,
                user.CareerLevel,
                user.ExperienceYears,
                phone = user.PhoneNumber,
                user.CreatedAt,
                user.ImagePath,
            });
        }

    }

}
