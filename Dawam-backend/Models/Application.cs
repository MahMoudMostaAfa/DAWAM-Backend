using System;
using System.ComponentModel.DataAnnotations;

namespace Dawam_backend.Models
{
    public class Application
    {
        public int Id { get; set; }

        [Required]
        public int JobId { get; set; }

        public Job Job { get; set; }

        [Required]
        public string UserId { get; set; }

        public ApplicationUser User { get; set; }

        [Range(0, 50, ErrorMessage = "Years of experience must be between 0 and 50.")]
        public int YearsOfExperience { get; set; }

        [Required]
        [MinLength(10, ErrorMessage = "Previous experience must be at least 10 characters.")]
        public string PreviousExperience { get; set; }

        [Range(0, 1000000, ErrorMessage = "Expected salary must be a positive number.")]
        public decimal ExpectedSalary { get; set; }

        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "The field must be exactly 11 characters.")]
        public string Phone { get; set; }

        [Required]
        [StringLength(255)]
        public string CVFilePath { get; set; }

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [RegularExpression("Pending|Accepted|Rejected", ErrorMessage = "Status must be Pending, Accepted, or Rejected.")]
        public string Status { get; set; } = "Pending";
    }
}
