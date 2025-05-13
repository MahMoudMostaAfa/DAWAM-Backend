using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Dawam_backend.DTOs.Applications
{
    public class ApplyForJobDto
    {
        [Required(ErrorMessage = "Job ID is required.")]
        public int JobId { get; set; }

        [Range(0, 100, ErrorMessage = "Years of experience cannot be negative or exceed 100 years.")]
        public int YearsOfExperience { get; set; }

        [MaxLength(500, ErrorMessage = "Previous experience description cannot exceed 500 characters.")]
        public string PreviousExperience { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Expected salary must be greater than zero.")]
        public decimal ExpectedSalary { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "Phone number must be exactly 11 digits.")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "CV file is required.")]
        public IFormFile CVFile { get; set; }
    }
}

