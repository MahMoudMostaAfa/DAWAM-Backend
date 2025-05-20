using Dawam_backend.DTOs.ForgetPassword;
using Dawam_backend.DTOs;
using System.Security.Claims;

namespace Dawam_backend.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResult> RegisterAsync(RegisterDto registerDto);
        Task<AuthResult> LoginAsync(LoginDto loginDto);
        Task<UserInfoResult> GetCurrentUserAsync(ClaimsPrincipal user);
        Task<AuthResult> UpdateProfileAsync(ClaimsPrincipal userPrincipal, UpdateProfileDto dto);
        Task<AuthResult> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<AuthResult> ResetPasswordAsync(ResetPasswordDto resetDto);
        Task<AuthResult> DeleteMeAsync(ClaimsPrincipal userPrincipal);

    }
}
