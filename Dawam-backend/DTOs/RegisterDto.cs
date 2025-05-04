using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; }
        [Required]
        [CustomRoleValidation(ErrorMessage = "Role must be either 'jobApplier' or 'jobPoster'.")]
        public string Role { get; set; } = "JobApplier";
        public IFormFile? Image { get; set; }

    }
    public class CustomRoleValidationAttribute : ValidationAttribute
    {
        private readonly string[] _validRoles = { "JobPoster", "JobApplier" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || !_validRoles.Contains(value.ToString()))
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
