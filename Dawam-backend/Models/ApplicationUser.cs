using Dawam_backend.Enums;
using Microsoft.AspNetCore.Identity;
namespace Dawam_backend.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; }= "JobApplier";
        public string? Title { get; set; }
        public string? Bio { get; set; }
      
        public string? Address { get; set; }
        public string? Location { get; set; }
        public CareerLevelE? CareerLevel { get; set; }
        public int? ExperienceYears { get; set; }
        public int? SubscriptionPlanId { get; set; }
        public SubscriptionPlan? SubscriptionPlan { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? ImagePath { get; set; }
    }
}
