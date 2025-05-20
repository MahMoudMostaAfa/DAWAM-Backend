using System.Net;
using Dawam_backend.Data;
using Dawam_backend.DTOs;
using Dawam_backend.DTOs.ForgetPassword;
using Dawam_backend.Helpers;
using Dawam_backend.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using Dawam_backend.Services.Interfaces;

namespace Dawam_backend.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtTokenHelper _jwtTokenHelper;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            JwtTokenHelper jwtTokenHelper,
            ApplicationDbContext context,
            IWebHostEnvironment env,
            IEmailService emailService,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _jwtTokenHelper = jwtTokenHelper;
            _context = context;
            _env = env;
            _emailService = emailService;
            _configuration = configuration;
        }

        public async Task<AuthResult> RegisterAsync(RegisterDto registerDto)
        {
            if (registerDto == null)
                return new AuthResult { Success = false, Message = "Invalid data" };

            string imagePath = "/ImagePath/Default.jpg";

            var user = new ApplicationUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FullName = registerDto.FullName,
                ImagePath = imagePath,
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
                return new AuthResult { Success = false, Errors = result.Errors };

            var baseSlug = SlugHelper.GenerateSlug(registerDto.FullName);
            user.Slug = await _context.Users.EnsureUniqueSlugAsync(baseSlug);

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return new AuthResult { Success = false, Errors = updateResult.Errors };
            }

            if (!string.IsNullOrEmpty(registerDto.Role))
                await _userManager.AddToRoleAsync(user, registerDto.Role);

            var token = await _jwtTokenHelper.GenerateJwtToken(user);

            return new AuthResult { Success = true, Message = "User registered successfully", Token = token };
        }

        public async Task<AuthResult> LoginAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null && !user.IsActive)
                return new AuthResult { Success = false, Message = "User not found or may be deleted" };

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
                return new AuthResult { Success = false, Message = "Invalid credentials" };

            var token = await _jwtTokenHelper.GenerateJwtToken(user);

            return new AuthResult { Success = true, Message = "Login successful", Token = token };
        }

        public async Task<UserInfoResult> GetCurrentUserAsync(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user == null) return null;

            var oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
            var isPremium = await _context.Payments.AnyAsync(p => p.UserId == user.Id && p.PaymentDate >= oneMonthAgo);
            var roles = await _userManager.GetRolesAsync(user);

            return new UserInfoResult
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Title = user.Title,
                Bio = user.Bio,
                Address = user.Address,
                Location = user.Location,
                CareerLevel = (int?)user.CareerLevel,
                ExperienceYears = user.ExperienceYears,
                Phone = user.PhoneNumber,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                Roles = roles,
                IsPremium = isPremium,
                ImagePath = user.ImagePath
            };
        }

        public async Task<AuthResult> UpdateProfileAsync(ClaimsPrincipal userPrincipal, UpdateProfileDto dto)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new AuthResult { Success = false, Message = "User not found" };

            if (!string.IsNullOrEmpty(dto.FullName)) user.FullName = dto.FullName;
            if (!string.IsNullOrEmpty(dto.Title)) user.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Bio)) user.Bio = dto.Bio;
            if (!string.IsNullOrEmpty(dto.Address)) user.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.Location)) user.Location = dto.Location;
            if (dto.CareerLevel.HasValue) user.CareerLevel = dto.CareerLevel.Value;
            if (dto.ExperienceYears.HasValue) user.ExperienceYears = dto.ExperienceYears.Value;
            if (!string.IsNullOrEmpty(dto.Phone)) user.PhoneNumber = dto.Phone;

            if (dto.Image != null)
            {
                var uploadsFolder = Path.Combine(_env.WebRootPath ?? "wwwroot", "ImagePath");
                Directory.CreateDirectory(uploadsFolder);

                var fileName = Guid.NewGuid() + Path.GetExtension(dto.Image.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                    await dto.Image.CopyToAsync(stream);

                if (!user.ImagePath.Contains("default.png"))
                {
                    var oldFilePath = Path.Combine(_env.WebRootPath ?? "wwwroot", user.ImagePath.TrimStart('/'));
                    if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
                }

                user.ImagePath = $"/ImagePath/{fileName}";
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? new AuthResult { Success = true, Message = "Profile updated successfully" }
                : new AuthResult { Success = false, Errors = result.Errors };
        }

        public async Task<AuthResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
            if (user == null || !user.IsActive)
                return new AuthResult { Success = true, Message = "Password reset link sent to email" };

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebUtility.UrlEncode(token);
            var resetUrl = $"{_configuration["EmailSettings:ClientURL"]}/reset-password?userId={user.Id}&token={encodedToken}";

            var emailBody = $@"<h1>Password Reset Request</h1>
                            <p>Click the link below to reset your password:</p>
                            <a href='{resetUrl}'>{resetUrl}</a>
                            <p>This link expires in 24 hours.</p>";

            await _emailService.SendEmailAsync(user.Email, "Password Reset Request", emailBody);
            return new AuthResult { Success = true, Message = "Password reset link sent to email" };
        }

        public async Task<AuthResult> ResetPasswordAsync(ResetPasswordDto resetDto)
        {
            var user = await _userManager.FindByIdAsync(resetDto.UserId);
            if (user == null || !user.IsActive)
                return new AuthResult { Success = false, Message = "Invalid request" };

            var result = await _userManager.ResetPasswordAsync(user, resetDto.Token, resetDto.NewPassword);
            return result.Succeeded
                ? new AuthResult { Success = true, Message = "Password reset successful" }
                : new AuthResult { Success = false, Errors = result.Errors };
        }

        public async Task<AuthResult> DeleteMeAsync(ClaimsPrincipal userPrincipal)
        {
            var userId = userPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new AuthResult { Success = false, Message = "User not found" };

            user.IsActive = false;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded
                ? new AuthResult { Success = true, Message = "Account deactivated successfully" }
                : new AuthResult { Success = false, Errors = result.Errors };
        }
    }
}