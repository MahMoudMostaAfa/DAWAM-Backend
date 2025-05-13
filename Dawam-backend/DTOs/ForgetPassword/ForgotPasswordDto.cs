using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.DTOs.ForgetPassword
{
    public class ForgotPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
