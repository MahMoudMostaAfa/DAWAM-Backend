using Dawam_backend.DTOs;
using Dawam_backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dawam_backend.Helpers;
using Microsoft.EntityFrameworkCore;
using Dawam_backend.Data;

namespace Dawam_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AuthController(UserManager<ApplicationUser> userManager,
                              SignInManager<ApplicationUser> signInManager,
                              IConfiguration configuration , JwtTokenHelper jwtTokenHelper, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _jwtTokenHelper = jwtTokenHelper;
            _context = context;
            _env = env;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (ModelState.IsValid)
            {
                // Save image if provided
                string imagePath = "/ImagePath/Default.jpg";

                var user = new ApplicationUser
                {
                    UserName = registerDto.Email,
                    Email = registerDto.Email,
                    FullName = registerDto.FullName,
                    ImagePath = imagePath, // Store image path in DB
                };

                var result = await _userManager.CreateAsync(user, registerDto.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(registerDto.Role))
                        await _userManager.AddToRoleAsync(user, registerDto.Role);

                    var token = await _jwtTokenHelper.GenerateJwtToken(user);

                    return Ok(new { message = "User registered successfully", token });
                }

                return BadRequest(result.Errors);
            }

            return BadRequest("Failed to register");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(loginDto.Email);

                if(user!=null && !user.IsActive)
                {
                    return NotFound(new { message = "this user not found or may be deleted" });
                }

                if (user != null && await _userManager.CheckPasswordAsync(user, loginDto.Password))
                {
                    // Generate JWT token
                    var token = await _jwtTokenHelper.GenerateJwtToken(user);

                    return Ok(new { message = "Login successful", token });
                }

                return Unauthorized(new { message = "Invalid credentials" });
            }

            return BadRequest(new { message = "Invalid data" });
        }


        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            // Get the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            var isPremiumFlag = await _context.Payments.AnyAsync(p => p.UserId == user.Id && p.PaymentDate >= oneMonthAgo);

            // Prepare user data to return
            var userData = new
            {
                user.Id,
                user.FullName,
                user.Email,
                user.Title,
                user.Bio,
                user.Address,
                user.Location,
                user.CareerLevel,
                user.ExperienceYears,
                phone=user.PhoneNumber,
                user.IsActive,
                user.CreatedAt,
                Roles = await _userManager.GetRolesAsync(user),
                isPremium = isPremiumFlag,
                user.ImagePath,
            };

            return Ok(userData);
        }

        [Authorize]
        [HttpPut("me")]
        [RequestSizeLimit(5_000_000)] // Limit photo size to 5MB
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return NotFound("User not found");

            // Update non-null fields
            if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.Title)) user.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Bio)) user.Bio = dto.Bio;
            if (!string.IsNullOrEmpty(dto.Address)) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.Location)) user.Location = dto.Location;
            if (dto.CareerLevel.HasValue) user.CareerLevel = dto.CareerLevel.Value;
            if (dto.ExperienceYears.HasValue) user.ExperienceYears = dto.ExperienceYears.Value;
            if (!string.IsNullOrEmpty(dto.Phone)) user.PhoneNumber = dto.Phone;

            // Handle new image upload
            if (dto.Image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "ImagePath");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.Image.CopyToAsync(stream);
                }

                // Optional: delete old image (if not default)
                if (!string.IsNullOrEmpty(user.ImagePath) && !user.ImagePath.Contains("default.png"))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath ?? "wwwroot", user.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                user.ImagePath = $"/ImagePath/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
                return Ok(new { message = "Profile updated successfully" });

            return BadRequest(result.Errors);
        }


        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // For JWT, logout is client-side — nothing to do server-side
            return Ok(new { message = "Logged out. Please clear token on client side." });
        }
        // Generate JWT Token

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteMe(){
            // Get current user ID from token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(new { message = "User not found in token." });
            }

            // Find user in the database
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Set IsActive to false
            user.IsActive = false;
            await _userManager.UpdateAsync(user);

            return Ok(new { message = "Your account has been deactivated." });
        }

        //    private async Task<string> GenerateJwtToken(ApplicationUser user)
        //    {
        //        // Prepare the claims (user info)
        //        var claims = new List<Claim>
        //{
        //    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
        //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //    new Claim(ClaimTypes.Name, user.FullName),
        //    new Claim(ClaimTypes.Email, user.Email),
        //};

        //        // Add user roles as claims
        //        var userRoles = await _userManager.GetRolesAsync(user);
        //        foreach (var role in userRoles)
        //        {
        //            claims.Add(new Claim(ClaimTypes.Role, role));
        //        }

        //        // Set up JWT key and signing credentials
        //        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //        // Create the JWT token
        //        var token = new JwtSecurityToken(
        //            issuer: _configuration["Jwt:Issuer"],
        //            audience: _configuration["Jwt:Audience"],
        //            claims: claims,
        //            expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpireInMinutes"])),
        //            signingCredentials: creds
        //        );

        //        return new JwtSecurityTokenHandler().WriteToken(token);
        //    }

    }
}
