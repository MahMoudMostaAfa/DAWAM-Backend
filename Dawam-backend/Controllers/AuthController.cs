using Dawam_backend.DTOs;
using Dawam_backend.DTOs.ForgetPassword;
using Dawam_backend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Dawam_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid) return BadRequest("Failed to register");

            var result = await _authService.RegisterAsync(registerDto);
            return result.Success
                ? Ok(new { message = result.Message, token = result.Token })
                : BadRequest(result.Errors);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid) return BadRequest(new { message = "Invalid data" });

            var result = await _authService.LoginAsync(loginDto);
            return result.Success
                ? Ok(new { message = result.Message, token = result.Token })
                : Unauthorized(new { message = result.Message });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userInfo = await _authService.GetCurrentUserAsync(User);
            return userInfo != null ? Ok(userInfo) : NotFound(new { message = "User not found" });
        }

        [Authorize]
        [HttpPut("me")]
        [RequestSizeLimit(5_000_000)]
        public async Task<IActionResult> UpdateProfile([FromForm] UpdateProfileDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.UpdateProfileAsync(User, dto);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(result.Errors);
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout() => Ok(new { message = "Logged out. Please clear token on client side." });

        [HttpDelete("me")]
        [Authorize]
        public async Task<IActionResult> DeleteMe()
        {
            var result = await _authService.DeleteMeAsync(User);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(result.Errors);
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid email format");

            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(result.Message);
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _authService.ResetPasswordAsync(resetDto);
            return result.Success ? Ok(new { message = result.Message }) : BadRequest(result.Errors);
        }
    }
}