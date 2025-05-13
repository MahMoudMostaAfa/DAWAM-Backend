using Dawam_backend.Enums;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Dawam_backend.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z]+$", ErrorMessage = "Username must contain only letters.")]
        [MaxLength(100, ErrorMessage = "Username cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression("^(JobApplier|JobPoster|Admin)$", ErrorMessage = "Invalid role.")]
        public string Role { get; set; } = "JobApplier";

        [MaxLength(100)]
        public string? Title { get; set; }

        [MaxLength(1000)]
        public string? Bio { get; set; }

        [MaxLength(200)]
        public string? Slug { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        [Required]
        public CareerLevelE CareerLevel { get; set; } = CareerLevelE.Junior;

        [Range(0, 50, ErrorMessage = "Experience must be between 0 and 50 years.")]
        public int? ExperienceYears { get; set; }

        public int? SubscriptionPlanId { get; set; }
        public SubscriptionPlan? SubscriptionPlan { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [MaxLength(300)]
        public string? ImagePath { get; set; }
    }
}
